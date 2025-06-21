using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform player; // assign player transform in Inspector or find in Start()
    public GameObject projectilePrefab;  // assign your projectile prefab here
    public float projectileSpeed = 10f;

    void Update()
    {
        RotateGunToMouse();

        if (Input.GetButtonDown("Fire1"))
        {
            FireProjectile();
        }
    }

    void RotateGunToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * projectileSpeed;  // updated for newer Unity versions
    }
}
