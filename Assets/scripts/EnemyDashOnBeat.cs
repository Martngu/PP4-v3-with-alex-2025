using System.Collections.Generic;
using UnityEngine;

public class DashingEnemy : MonoBehaviour
{
    public Transform player;
    public float dashDistance = 2f;
    public float dashSpeed = 10f;
    public Transform spawnPoint;

    public float separationRadius = 1f;
    public float separationStrength = 2f;

    private bool isDashing = false;
    private Vector3 dashTarget;

    private void OnEnable()
    {
        BeatTracker.OnMove += HandleMove;
    }

    private void OnDisable()
    {
        BeatTracker.OnMove -= HandleMove;
    }

    private void HandleMove()
    {
        if (player != null && GameManager.Instance.isPlayerAlive)
        {
            Vector3 chaseDirection = (player.position - transform.position).normalized;

            Vector3 separationDirection = GetSeparationDirection();
            Vector3 finalDirection = (chaseDirection + separationDirection * separationStrength).normalized;

            dashTarget = transform.position + finalDirection * dashDistance;
            isDashing = true;
        }
    }

    private void Update()
    {
        if (isDashing)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, dashSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, dashTarget) < 0.1f)
            {
                isDashing = false;
            }
        }
    }

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
                    separation += away.normalized / away.magnitude;
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
            GameManager.Instance.PlayerDied();
            collision.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
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
        BeatTracker beatTracker = FindFirstObjectByType<BeatTracker>();
        if (beatTracker != null)
        {
            beatTracker.ReturnSpawnPoint(spawnPoint);
        }

        Destroy(gameObject);
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }
}
