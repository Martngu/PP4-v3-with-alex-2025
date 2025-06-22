using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 moveDirection;

    public void SetTarget(Transform targetTransform)
    {
        // Calculate direction to player at spawn time and fix it
        moveDirection = (targetTransform.position - transform.position).normalized;

        // Rotate projectile to face the player position at spawn time
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);  // Adjust if your sprite faces up by default
    }

    void Update()
    {
        if (!GameManager.Instance.isPlayerAlive)
        {
            // Stop moving when player is dead
            return;
        }

        // Move straight in the fixed direction
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit by ProjectileEnemy!");

            GameManager.Instance.PlayerDied();
            collision.gameObject.SetActive(false);

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle") || collision.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Destroy(gameObject, 10f);
    }
}
