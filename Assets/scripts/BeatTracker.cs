using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class BeatTracker : MonoBehaviour
{
    public EventReference musicEvent;
    private EventInstance musicInstance;

    FMOD.Studio.EVENT_CALLBACK markerCallback;

    [StructLayout(LayoutKind.Sequential)]
    public struct TimelineMarkerProperties
    {
        public IntPtr name;
        public int position;
    }

    public GameObject enemyPrefab;       // Assign in Inspector
    public Transform[] spawnPoints;      // Assign multiple spawn points in Inspector

    private List<Transform> availableSpawnPoints = new List<Transform>();
    private bool spawnRequest = false;

    void Start()
    {
        // Create and start music instance
        musicInstance = RuntimeManager.CreateInstance(musicEvent);

        // Set callback for timeline markers
        markerCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        musicInstance.setCallback(markerCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        musicInstance.start();

        // Fill available spawn points list
        availableSpawnPoints.AddRange(spawnPoints);
    }

    void Update()
    {
        if (spawnRequest)
        {
            spawnRequest = false;
            SpawnEnemy();
        }
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);
            TimelineMarkerProperties marker = (TimelineMarkerProperties)Marshal.PtrToStructure(parameterPtr, typeof(TimelineMarkerProperties));
            string markerName = Marshal.PtrToStringAnsi(marker.name);

            if (markerName == "Beat")
            {
                Debug.Log("Beat detected at position: " + marker.position);
                spawnRequest = true;
            }
        }

        return FMOD.RESULT.OK;
    }

    void SpawnEnemy()
    {
        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No available spawn points left!");
            return;
        }

        // Pick a random available spawn point
        int randomIndex = UnityEngine.Random.Range(0, availableSpawnPoints.Count);
        Transform chosenSpawnPoint = availableSpawnPoints[randomIndex];

        // Spawn enemy
        GameObject enemy = Instantiate(enemyPrefab, chosenSpawnPoint.position, Quaternion.identity);

        // Assign player to the enemy
        enemy.GetComponent<EnemyChase>().player = GameObject.FindWithTag("Player").transform;

        // Optional: Remember which spawn point this enemy used
        EnemyChase enemyChase = enemy.GetComponent<EnemyChase>();
        enemyChase.player = GameObject.FindWithTag("Player").transform;
        enemyChase.spawnPoint = chosenSpawnPoint; // Needed if you want to free the spawn point later

        // Remove the spawn point from the list
        availableSpawnPoints.RemoveAt(randomIndex);

        Debug.Log("Enemy spawned on beat at " + chosenSpawnPoint.position);
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }
    public void ReturnSpawnPoint(Transform spawnPoint)
    {
        availableSpawnPoints.Add(spawnPoint);
    }
}
