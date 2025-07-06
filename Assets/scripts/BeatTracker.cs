using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class BeatTracker : MonoBehaviour
{
    public static event Action OnBeat;
    public static event Action OnReloadBeat;
    public static event Action OnMove;

    public EventReference musicEvent;
    private EventInstance musicInstance;

    FMOD.Studio.EVENT_CALLBACK markerCallback;

    [StructLayout(LayoutKind.Sequential)]
    public struct TimelineMarkerProperties
    {
        public IntPtr name;
        public int position;
    }

    public GameObject enemyPrefab;
    public GameObject dashEnemyPrefab;
    public GameObject warningPrefab;
    public Transform[] spawnPoints;

    private int currentSpawnIndex = 0;
    private bool spawnRequest = false;

    private List<Transform> availableSpawnPoints = new List<Transform>();

    void Start()
    {
        availableSpawnPoints.AddRange(spawnPoints);

        musicInstance = RuntimeManager.CreateInstance(musicEvent);

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

            if (markerName.StartsWith("beat", StringComparison.OrdinalIgnoreCase) && !markerName.StartsWith("beat_reload", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Beat detected at position: " + marker.position + " markerName: " + markerName);
                spawnRequest = true;
                OnBeat?.Invoke();
            }

            if (markerName.StartsWith("beat_reload", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Reload Beat detected at position: " + marker.position + " markerName: " + markerName);
                OnReloadBeat?.Invoke();
            }

            if (markerName.StartsWith("move", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Move marker detected at position: " + marker.position + " markerName: " + markerName);
                OnMove?.Invoke();
            }

            if (markerName.StartsWith("spawn_warning", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Spawn warning at marker: " + markerName);
                ShowSpawnWarning();
            }
            if (markerName.Equals("end", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("END marker reached — level complete!");
                GameManager.Instance.LevelComplete();
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
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;

        bool spawnDashEnemy = UnityEngine.Random.value > 0.5f;
        GameObject enemy;

        if (spawnDashEnemy)
        {
            enemy = Instantiate(dashEnemyPrefab, chosenSpawnPoint.position, Quaternion.identity);

            DashingEnemy dashEnemy = enemy.GetComponent<DashingEnemy>();
            dashEnemy.SetTarget(GameObject.FindWithTag("Player").transform);

            Debug.Log("Dash enemy spawned on beat at " + chosenSpawnPoint.position);
        }
        else
        {
            enemy = Instantiate(enemyPrefab, chosenSpawnPoint.position, Quaternion.identity);

            EnemyChase enemyChase = enemy.GetComponent<EnemyChase>();
            enemyChase.player = GameObject.FindWithTag("Player").transform;
            enemyChase.spawnPoint = chosenSpawnPoint;

            Debug.Log("Chase enemy spawned on beat at " + chosenSpawnPoint.position);
        }
    }

    void ShowSpawnWarning()
    {
        if (spawnPoints.Length == 0 || warningPrefab == null)
        {
            Debug.LogWarning("No spawn points or warning prefab assigned.");
            return;
        }

        Transform warningPoint = spawnPoints[currentSpawnIndex];
        Instantiate(warningPrefab, warningPoint.position, Quaternion.identity);
    }

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
    public EventInstance GetMusicInstance()
    {
        return musicInstance;
    }
}
