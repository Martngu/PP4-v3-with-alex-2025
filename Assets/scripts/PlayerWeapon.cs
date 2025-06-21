using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer; // Set in Inspector to only hit "Enemy" layer

    private float lastAttackTime = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        // Detect enemies in attack range using OverlapCircle
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            EnemyChase enemy = hit.GetComponent<EnemyChase>();
            if (enemy != null)
            {
                enemy.TakeDamage();
            }
        }
    }

    // Optional: visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
