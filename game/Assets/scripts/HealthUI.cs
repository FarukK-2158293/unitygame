using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Image[] healthHearts;
    public Text healthText; // Optional text display

    [Header("Heart Colors")]
    public Color fullHealthColor = Color.red;
    public Color emptyHealthColor = Color.gray;

    private PlayerStats playerStats;

    void Start()
    {
        // Find the player stats component
        playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("HealthUI: Could not find PlayerStats component!");
            return;
        }

        // Subscribe to health change events
        playerStats.OnHealthChanged.AddListener(UpdateHealthDisplay);

        // Initial health display
        UpdateHealthDisplay(playerStats.GetCurrentHealth());
    }

    void UpdateHealthDisplay(int currentHealth)
    {
        // Update heart images
        for (int i = 0; i < healthHearts.Length; i++)
        {
            if (healthHearts[i] != null)
            {
                if (i < currentHealth)
                {
                    healthHearts[i].color = fullHealthColor;
                }
                else
                {
                    healthHearts[i].color = emptyHealthColor;
                }
            }
        }

        // Update text if it exists
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}/{playerStats.GetMaxHealth()}";
        }

        Debug.Log($"Health UI updated: {currentHealth} hearts displayed");
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (playerStats != null)
        {
            playerStats.OnHealthChanged.RemoveListener(UpdateHealthDisplay);
        }
    }
}
