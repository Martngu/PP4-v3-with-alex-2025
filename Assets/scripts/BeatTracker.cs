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

    private int currentSpawnIndex = 0;
    private bool spawnRequest = false;

    private List<Transform> availableSpawnPoints = new List<Transform>();

    void Start()
    {
        // Initialize available spawn points list
        availableSpawnPoints.AddRange(spawnPoints);

        // Create and start music instance
        musicInstance = RuntimeManager.CreateInstance(musicEvent);

        // Set callback for timeline markers
        markerCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        musicInstance.setCallback(markerCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        musicInstance.start();
    }

    void Update()
    {
        if (spawnRequest && GameManager.Instance.isPlayerAlive)
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

            if (markerName.StartsWith("beat", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Beat detected at position: " + marker.position + " markerName: " + markerName);
                spawnRequest = true;
            }
        }

        return FMOD.RESULT.OK;
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned.");
            return;
        }

        Transform chosenSpawnPoint = spawnPoints[currentSpawnIndex];

        // Cycle to next spawn point
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;

        GameObject enemy = Instantiate(enemyPrefab, chosenSpawnPoint.position, Quaternion.identity);

        EnemyChase enemyChase = enemy.GetComponent<EnemyChase>();
        enemyChase.player = GameObject.FindWithTag("Player").transform;
        enemyChase.spawnPoint = chosenSpawnPoint;

        Debug.Log("Enemy spawned on beat at " + chosenSpawnPoint.position);
    }

    // Called by the enemy when it is destroyed
    public void ReturnSpawnPoint(Transform spawnPoint)
    {
        if (!availableSpawnPoints.Contains(spawnPoint))
        {
            availableSpawnPoints.Add(spawnPoint);
        }
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }
    public void StopMusic()
    {
        if (musicInstance.isValid())
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }
    }
}
