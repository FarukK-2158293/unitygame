using UnityEngine;

public class spiderScript : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 3f;

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
    private LayerMask defaultLayerMask = 1 << 0; // Default layer (layer 0)

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
    }

    void Update()
    {
        if (hasDropped || rb == null) return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, detectionRange, defaultLayerMask);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * detectionRange);
    }
}
