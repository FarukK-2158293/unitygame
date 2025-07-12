using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    public float attackDamage = 10f;
    public float attackRange = 15.0f;
    public float attackCooldown = 0.6f;
    public LayerMask enemyLayers;

    [Header("Attack Point")]
    public Transform attackPoint;

    private Animator animator;
    private PlayerStats playerStats;
    private float nextAttackTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();

        if (attackPoint == null)
            attackPoint = transform;
    }

    void Update()
    {
        if (playerStats != null && playerStats.IsAlive())
        {
            HandleCombatInput();
        }
    }

    void HandleCombatInput()
    {
        if ((Mouse.current.leftButton.wasPressedThisFrame)
            && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        animator.SetTrigger("isAttacking");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log($"Hit {enemy.name} for {attackDamage} damage!");

        }
    }
}
