using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;

public class BeatTracker2 : MonoBehaviour
{
    public static event Action OnBeat;           // Fires on every beat
    public static event Action OnReloadBeat;     // Fires on reload-specific beats

    public EventReference musicEvent;
    private EventInstance musicInstance;

    FMOD.Studio.EVENT_CALLBACK markerCallback;

    [StructLayout(LayoutKind.Sequential)]
    public struct TimelineMarkerProperties
    {
        public IntPtr name;
        public int position;
    }

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public GameObject dashEnemyPrefab;
    public Transform[] enemySpawnPoints;

    private int currentEnemySpawnIndex = 0;
    private bool enemySpawnRequest = false;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform player;
    public Transform[] projectileSpawnPoints;

    private int currentProjectileSpawnIndex = 0;
    private bool projectileSpawnRequest = false;

    [Header("Laser Settings")]
    public GameObject laserProjectilePrefab; // Assign prefab with LaserProjectile script

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);

        markerCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        musicInstance.setCallback(markerCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);

        musicInstance.start();
    }

    void Update()
    {
        if (!GameManager.Instance.isPlayerAlive)
        {
            // Stop spawning and music if player is dead
            enemySpawnRequest = false;
            projectileSpawnRequest = false;
            StopMusic();
            return;
        }

        if (enemySpawnRequest)
        {
            enemySpawnRequest = false;
            SpawnEnemy();
        }

        if (projectileSpawnRequest)
        {
            projectileSpawnRequest = false;
            SpawnProjectile();
        }
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
        {
            TimelineMarkerProperties marker = (TimelineMarkerProperties)Marshal.PtrToStructure(parameterPtr, typeof(TimelineMarkerProperties));
            string markerName = Marshal.PtrToStringAnsi(marker.name);

            if (markerName.StartsWith("beat", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Beat detected at position: " + marker.position + " markerName: " + markerName);

                enemySpawnRequest = true;
                projectileSpawnRequest = true;

                OnBeat?.Invoke();
            }

            if (markerName.StartsWith("beat_reload", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Reload Beat detected at position: " + marker.position + " markerName: " + markerName);

                OnReloadBeat?.Invoke();

                SpawnLaser();
            }
        }

        return FMOD.RESULT.OK;
    }

    void SpawnEnemy()
    {
        if (enemySpawnPoints.Length == 0)
        {
            Debug.LogWarning("No enemy spawn points assigned.");
            return;
        }

        Transform spawnPoint = enemySpawnPoints[currentEnemySpawnIndex];
        currentEnemySpawnIndex = (currentEnemySpawnIndex + 1) % enemySpawnPoints.Length;

        bool spawnDashEnemy = UnityEngine.Random.value > 0.5f;
        GameObject enemy;

        if (spawnDashEnemy)
        {
            enemy = Instantiate(dashEnemyPrefab, spawnPoint.position, Quaternion.identity);
            DashingEnemy dashEnemy = enemy.GetComponent<DashingEnemy>();
            dashEnemy.SetTarget(player);

            Debug.Log("Dash enemy spawned at " + spawnPoint.position);
        }
        else
        {
            enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyChase enemyChase = enemy.GetComponent<EnemyChase>();
            enemyChase.player = player;
            enemyChase.spawnPoint = spawnPoint;

            Debug.Log("Chase enemy spawned at " + spawnPoint.position);
        }
    }

    void SpawnProjectile()
    {
        if (projectileSpawnPoints.Length == 0)
        {
            Debug.LogWarning("No projectile spawn points assigned.");
            return;
        }

        Transform spawnPoint = projectileSpawnPoints[currentProjectileSpawnIndex];
        currentProjectileSpawnIndex = (currentProjectileSpawnIndex + 1) % projectileSpawnPoints.Length;

        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        ProjectileEnemy projectile = proj.GetComponent<ProjectileEnemy>();

        if (projectile != null)
        {
            projectile.SetTarget(player);
        }

        Debug.Log("Projectile spawned at " + spawnPoint.position);
    }

    void SpawnLaser()
    {
        if (laserProjectilePrefab == null)
        {
            Debug.LogWarning("Laser projectile prefab not assigned.");
            return;
        }

        // Example: spawn laser at random position around player at radius 8
        float radius = 8f;
        float angle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
        Vector3 spawnPos = player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;

        GameObject laserGO = Instantiate(laserProjectilePrefab, spawnPos, Quaternion.identity);
        LaserProjectile laser = laserGO.GetComponent<LaserProjectile>();

        if (laser != null)
        {
            laser.Initialize(spawnPos, player.position, player);
        }

        Debug.Log("Laser spawned at " + spawnPos);
    }

    void OnDestroy()
    {
        StopMusic();
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
