using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public Transform spawnPoint;

    private void Update()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player caught!");
            // You can add a game over handler or destroy the player here
            Destroy(collision.gameObject);
        }
    }

    private void OnDestroy()
    {
        BeatTracker beatTracker = FindFirstObjectByType<BeatTracker>();
        if (beatTracker != null)
        {
            beatTracker.ReturnSpawnPoint(spawnPoint);
        }
    }
}
