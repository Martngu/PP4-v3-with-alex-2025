using UnityEngine;
using UnityEngine.UI;

public class GunController : MonoBehaviour
{
    public Transform player; // Assign player transform
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    public float normalFireRate = 1f; // shots per second
    public float fastFireRate = 3f;   // shots per second if on beat
    public float beatWindow = 0.3f;   // seconds after beat where firing counts as on-beat

    public Image beatIndicator;       // UI element for beat feedback

    private float fireCooldown = 0f;
    private float beatTimer = 0f;

    void OnEnable()
    {
        BeatTracker.OnBeat += OnBeat; // Subscribe to beat event
    }

    void OnDisable()
    {
        BeatTracker.OnBeat -= OnBeat; // Unsubscribe to avoid memory leaks
    }

    void Update()
    {
        RotateGunToMouse();

        fireCooldown -= Time.deltaTime;
        beatTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && fireCooldown <= 0f)
        {
            FireProjectile();
        }

        // Fade beat indicator out smoothly
        if (beatIndicator != null)
        {
            beatIndicator.color = Color.Lerp(beatIndicator.color, Color.clear, Time.deltaTime * 5f);
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
        float currentFireRate = normalFireRate;

        if (beatTimer > 0f)
        {
            currentFireRate = fastFireRate;
            beatTimer = 0f; // Reset so player can’t chain fast shots

            if (beatIndicator != null)
            {
                beatIndicator.color = Color.green; // Flash green on perfect timing
            }
        }
        else
        {
            if (beatIndicator != null)
            {
                beatIndicator.color = Color.white; // Normal fire flash
            }
        }

        // Instantiate bullet and set velocity
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * projectileSpeed;

        // Set cooldown based on fire rate
        fireCooldown = 1f / currentFireRate;
    }

    void OnBeat()
    {
        beatTimer = beatWindow;

        if (beatIndicator != null)
        {
            beatIndicator.color = Color.white; // Flash UI white on beat
        }
    }
}
