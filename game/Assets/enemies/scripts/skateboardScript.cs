using UnityEngine;

public class skateboardScript : MonoBehaviour
{
  [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public LayerMask playerLayerMask = 1;

    [Header("Launch Settings")]
    public float launchForce = 50f;
    public float launchCooldown = 2f;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    private GameObject player;
    private Rigidbody2D rb;
    private float lastLaunchTime = 0f;
    private bool playerInRange = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject;
        }
        else
        {
            Debug.LogWarning("No GameObject with 'Player' tag found!");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRadius)
        {
            if (!playerInRange)
            {
                playerInRange = true;
            }

            if (CanLaunch())
            {
                LaunchTowardsPlayer();
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                rb.linearVelocity = Vector2.zero; // Corrected from linearVelocity
            }
        }
    }

    bool CanLaunch()
    {
        return Time.time >= lastLaunchTime + launchCooldown;
    }

    void LaunchTowardsPlayer()
    {
        if (player == null || rb == null) return;

        Vector2 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0f;
        direction.Normalize();

        rb.linearVelocity = Vector2.zero; // Corrected from linearVelocity
        rb.AddForce(direction * launchForce, ForceMode2D.Impulse);

        lastLaunchTime = Time.time;

        Debug.Log("Monster launched horizontally towards player!");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Monster hit the player and will be destroyed!");
            Destroy(gameObject); // Destroy this monster
        }
    }

}
