using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;
    public bool destroyAfterHit = false;
    public bool requiresPlayerTag = true;

    [Header("Damage Timing")]
    public float damageInterval = 0.1f; // How often to check for damage (every 0.1 seconds)

    [Header("Visual Feedback")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private float lastDamageTime = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    // This fires every frame while player is touching the collider
    void OnTriggerStay2D(Collider2D other)
    {
        // Only check for damage at intervals (not every frame)
        if (Time.time >= lastDamageTime + damageInterval)
        {
            HandleCollision(other.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Only check for damage at intervals (not every frame)
        if (Time.time >= lastDamageTime + damageInterval)
        {
            HandleCollision(collision.gameObject);
        }
    }

    void HandleCollision(GameObject other)
    {
        // Check if it's the player (by tag or component)
        bool isPlayer = false;

        if (requiresPlayerTag)
        {
            isPlayer = other.CompareTag("Player");
        }
        else
        {
            isPlayer = other.GetComponent<PlayerStats>() != null;
        }

        if (isPlayer)
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                bool damageApplied = playerStats.TakeDamage(damageAmount);

                if (damageApplied)
                {
                    lastDamageTime = Time.time; // Record when damage was applied
                    StartCoroutine(FlashEffect());
                    Debug.Log($"Damage collider dealt {damageAmount} damage to player!");

                    if (destroyAfterHit)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }
}
