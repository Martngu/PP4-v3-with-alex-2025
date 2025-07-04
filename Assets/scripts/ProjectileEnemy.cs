using UnityEngine;

public class ProjectileEnemy : MonoBehaviour
{
    public float speed = 5f;
    public float boundaryIgnoreTime = 0.2f; 

    private Vector3 moveDirection;
    private float spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
        Destroy(gameObject, 10f); 
    }

    public void SetTarget(Transform targetTransform)
    {
        moveDirection = (targetTransform.position - transform.position).normalized;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void Update()
    {
        if (!GameManager.Instance.isPlayerAlive) return;

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            if (Time.time - spawnTime > boundaryIgnoreTime)
            {
                Destroy(gameObject);
            }

            return;
        }

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit by ProjectileEnemy!");
            GameManager.Instance.PlayerDied();
            collision.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
