using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    public float previewDuration = 1.5f;
    public float laserDuration = 2.0f;
    public float laserLength = 10f;
    public float laserWidth = 0.5f;

    private SpriteRenderer laserSprite;
    private BoxCollider2D boxCollider;

    private bool isFiring = false;

    private Transform player;

    void Awake()
    {
        laserSprite = GetComponentInChildren<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (laserSprite == null)
            Debug.LogError("LaserProjectile needs a child with SpriteRenderer.");

        if (boxCollider == null)
            Debug.LogError("LaserProjectile needs a BoxCollider2D.");

        boxCollider.enabled = false;
    }

    public void Initialize(Vector3 startPosition, Vector3 targetPosition, Transform playerTransform)
    {
        player = playerTransform;

        transform.position = startPosition;

        Vector3 direction = (targetPosition - startPosition).normalized;

        // Rotate laser parent to face target direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Adjust the sprite scale: x is length, y is width
        laserSprite.transform.localScale = new Vector3(laserLength, laserWidth, 1);

        // Position the sprite so that its left edge starts at local position 0
        laserSprite.transform.localPosition = new Vector3(laserLength / 2f, 0, 0);

        // Setup BoxCollider to match sprite size and position
        boxCollider.size = new Vector2(laserLength, laserWidth);
        boxCollider.offset = new Vector2(laserLength / 2f, 0);

        StartCoroutine(LaserRoutine());
    }

    private System.Collections.IEnumerator LaserRoutine()
    {
        // Preview: make the sprite semi-transparent
        Color previewColor = new Color(1, 0, 0, 0.3f);
        laserSprite.color = previewColor;

        yield return new WaitForSeconds(previewDuration);

        // Fire laser: solid color and enable collider
        laserSprite.color = Color.red;
        isFiring = true;
        boxCollider.enabled = true;

        yield return new WaitForSeconds(laserDuration);

        boxCollider.enabled = false;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isFiring) return;

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit by Laser!");

            GameManager.Instance.PlayerDied();
            collision.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.isPlayerAlive)
        {
            Destroy(gameObject);
        }
    }
}
