using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public float immunityDuration = 2f;

    [Header("Status Effects")]
    public bool canTakeDamage = true;
    public bool isAlive = true;

    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;
    public UnityEvent OnPlayerDied;
    public UnityEvent OnDamageTaken;
    public UnityEvent OnImmunityStarted;
    public UnityEvent OnImmunityEnded;

    private int currentHealth;
    private bool isImmune = false;
    private float immunityTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public static PlayerStats Instance { get; private set; }

    void Awake()
    {
        // Singleton pattern for easy access
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log($"Player stats initialized: {currentHealth}/{maxHealth} health");
    }

    void Update()
    {
        HandleImmunity();
    }

    void HandleImmunity()
    {
        if (isImmune)
        {
            immunityTimer -= Time.deltaTime;

            // Visual feedback - blink during immunity
            if (spriteRenderer != null)
            {
                float alpha = Mathf.PingPong(Time.time * 10f, 1f);
                Color color = originalColor;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            if (immunityTimer <= 0f)
            {
                EndImmunity();
            }
        }
    }

    public bool TakeDamage(int damageAmount = 1)
    {
        if (!canTakeDamage || isImmune || !isAlive)
            return false;

        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        StartImmunity();

        OnHealthChanged?.Invoke(currentHealth);
        OnDamageTaken?.Invoke();

        Debug.Log($"Player took {damageAmount} damage! Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

        return true; // Damage was successfully applied
    }

    public void Heal(int healAmount = 1)
    {
        if (!isAlive) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        OnHealthChanged?.Invoke(currentHealth);

        Debug.Log($"Player healed {healAmount}! Health: {currentHealth}/{maxHealth}");
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log("Player health fully restored!");
    }

    void StartImmunity()
    {
        isImmune = true;
        immunityTimer = immunityDuration;
        OnImmunityStarted?.Invoke();
    }

    void EndImmunity()
    {
        isImmune = false;

        // Restore full opacity
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        OnImmunityEnded?.Invoke();
    }

    void Die()
    {
        // Disable all colliders when player dies
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            // Only modify BoxCollider2D types
            if (col is BoxCollider2D boxCollider)
            {
                boxCollider.size = new Vector2(0.3f, 0.3f);
                boxCollider.offset = new Vector2(0.005f, 0.17f);
            }
        }

        isAlive = false;
        canTakeDamage = false;
        OnPlayerDied?.Invoke();
        Debug.Log("Player died!");
    }

    public void Revive()
    {
        isAlive = true;
        canTakeDamage = true;
        currentHealth = maxHealth;
        EndImmunity();
        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log("Player revived!");
    }

    // Getters
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsImmune() => isImmune;
    public bool IsAlive() => isAlive;
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
}
