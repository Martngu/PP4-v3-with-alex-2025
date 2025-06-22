using UnityEngine;

public class ProjectileSpawner2 : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform player;
    public float spawnRadius = 8f;

    private void OnEnable()
    {
        BeatTracker2.OnBeat += SpawnProjectile;
    }

    private void OnDisable()
    {
        BeatTracker2.OnBeat -= SpawnProjectile;
    }

    void SpawnProjectile()
    {
        Vector3 spawnPosition = GetSpawnPosition();

        GameObject proj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        ProjectileEnemy projectile = proj.GetComponent<ProjectileEnemy>();

        if (projectile != null)
        {
            projectile.SetTarget(player);
        }
    }

    Vector3 GetSpawnPosition()
    {
        // Spawn on a circle radius around the player
        float angle = Random.Range(0f, 2 * Mathf.PI);
        Vector3 spawnPos = player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * spawnRadius;
        return spawnPos;
    }
}
