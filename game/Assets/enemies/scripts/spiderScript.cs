using UnityEngine;

public class spiderScript : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 1f;
    public float horizontalDetectionDistance = 0.5f; // New: horizontal detection distance

    [Header("Drop Settings")]
    public float dropForce = 10f;
    public bool destroyAfterHit = true;

    [Header("Damage Settings")]
    public int damageAmount = 1;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool hasDropped = false;
    private bool hasDamaged = false;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Spider is missing Rigidbody2D!");
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        // Find the player GameObject
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (hasDropped || rb == null || player == null) return;

        // Check horizontal distance to player
        float horizontalDistance = Mathf.Abs(transform.position.x - player.position.x);

        // Drop if player is within horizontal range and below the spider
        if (horizontalDistance <= horizontalDetectionDistance)
        {
            DropOnPlayer();
        }
    }

    void DropOnPlayer()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.AddForce(Vector2.down * dropForce, ForceMode2D.Impulse);
        hasDropped = true;
        Debug.Log("Spider dropped on player!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasDamaged) return;

        if (collision.collider.CompareTag("Player"))
        {
            PlayerStats stats = collision.collider.GetComponent<PlayerStats>();
            if (stats != null)
            {
                bool damageApplied = stats.TakeDamage(damageAmount);
                if (damageApplied)
                {
                    hasDamaged = true;
                    Debug.Log("Spider damaged player!");
                    StartCoroutine(FlashAndDestroy());
                }
            }
        }
    }

    System.Collections.IEnumerator FlashAndDestroy()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }

        if (destroyAfterHit)
        {
            Destroy(gameObject);
        }
    }

}
