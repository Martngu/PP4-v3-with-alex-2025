using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public Transform spawnPoint;
    public float separationRadius = 1f;
    public float separationStrength = 2f;

    private void Update()
    {
        // Stop moving if player is dead
        if (player != null && GameManager.Instance.isPlayerAlive)
        {
            Vector3 chaseDirection = (player.position - transform.position).normalized;

            // Apply separation
            Vector3 separationDirection = GetSeparationDirection();

            // Combined movement: chase + separation
            Vector3 finalDirection = (chaseDirection + separationDirection * separationStrength).normalized;

            transform.position += finalDirection * speed * Time.deltaTime;
        }
    }

    // Separation behavior: gently move away from nearby enemies
    private Vector3 GetSeparationDirection()
    {
        Vector3 separation = Vector3.zero;
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (Collider2D collider in nearbyEnemies)
        {
            if (collider.gameObject != this.gameObject && collider.CompareTag("Enemy"))
            {
                Vector3 away = transform.position - collider.transform.position;
                if (away != Vector3.zero)
                {
                    separation += away.normalized / away.magnitude; // closer enemies push harder
                }
            }
        }

        return separation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && GameManager.Instance.isPlayerAlive)
        {
            Debug.Log("Player caught!");

            // Call GameManager to handle death
            GameManager.Instance.PlayerDied();

            // Disable the player
            collision.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }

    private void OnDestroy()
    {
        BeatTracker beatTracker = FindFirstObjectByType<BeatTracker>();
        if (beatTracker != null)
        {
            beatTracker.ReturnSpawnPoint(spawnPoint);
        }
    }
    public void TakeDamage()
    {
        // Play enemy death effects here (particles, sound, animation)...

        // Return spawn point to BeatTracker so that it can be reused
        BeatTracker beatTracker = FindFirstObjectByType<BeatTracker>();
        if (beatTracker != null)
        {
            beatTracker.ReturnSpawnPoint(spawnPoint);
        }

        Destroy(gameObject);
    }
}
