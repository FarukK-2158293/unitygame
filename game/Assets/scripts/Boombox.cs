using UnityEngine;
using System.Collections;

public enum BoomboxState
{
    Idle,
    Walking,
    Attacking
}

public class Boombox : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;
    public float rotationSpeed = 5f;

    [Header("Behavior Settings")]
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    public float minWalkTime = 2f;
    public float maxWalkTime = 5f;
    public float attackChance = 0.2f; // 20% chance to attack when changing states
    public float attackDuration = 2f;

    [Header("Area Constraint")]
    public PatrolArea patrolArea;
    public float areaCheckDistance = 1f;

    [Header("Animation Settings")]
    public float idleAnimationSpeed = 1f;
    public float walkAnimationSpeed = 1f;
    public float attackAnimationSpeed = 1f;

    private BoomboxState currentState;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 targetPosition;
    private float stateTimer;
    private bool isMovingToTarget;

    // Animation parameter names (adjust these to match your animator)
    private readonly string SPEED_PARAM = "Speed";
    private readonly string IS_WALKING_PARAM = "isWalking";
    private readonly string IS_ATTACKING_PARAM = "isAttacking";
    private readonly string ATTACK_TRIGGER = "Attack";

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (patrolArea == null)
        {
            patrolArea = FindObjectOfType<PatrolArea>();
            if (patrolArea == null)
            {
                GameObject areaGO = new GameObject("BoomboxPatrolArea");
                patrolArea = areaGO.AddComponent<PatrolArea>();
                Debug.LogWarning("No PatrolArea found, created a default one. Please configure it in the scene.");
            }
        }

        ChangeState(BoomboxState.Idle);
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BoomboxState.Idle:
                HandleIdleState();
                break;
            case BoomboxState.Walking:
                HandleWalkingState();
                break;
            case BoomboxState.Attacking:
                HandleAttackingState();
                break;
        }

        CheckAreaConstraints();
        UpdateAnimatorParameters();
    }

    void HandleIdleState()
    {
        // Only stop horizontal movement, keep vertical velocity for gravity
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (stateTimer <= 0)
        {
            // Random chance to attack or walk
            if (Random.value < attackChance)
            {
                ChangeState(BoomboxState.Attacking);
            }
            else
            {
                ChangeState(BoomboxState.Walking);
            }
        }
    }

    void HandleWalkingState()
    {
        if (!isMovingToTarget || Mathf.Abs(transform.position.x - targetPosition.x) < 0.5f)
        {
            SetNewTargetPosition();
        }

        MoveTowardsTarget();

        if (stateTimer <= 0)
        {
            // Random chance to attack or idle
            if (Random.value < attackChance)
            {
                ChangeState(BoomboxState.Attacking);
            }
            else
            {
                ChangeState(BoomboxState.Idle);
            }
        }
    }

    void HandleAttackingState()
    {
        // Only stop horizontal movement, keep vertical velocity for gravity
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (stateTimer <= 0)
        {
            // After attacking, randomly choose next state
            if (Random.value < 0.5f)
            {
                ChangeState(BoomboxState.Idle);
            }
            else
            {
                ChangeState(BoomboxState.Walking);
            }
        }
    }

    void ChangeState(BoomboxState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case BoomboxState.Idle:
                stateTimer = Random.Range(minIdleTime, maxIdleTime);
                SetAnimatorState(false, false, idleAnimationSpeed);
                break;

            case BoomboxState.Walking:
                stateTimer = Random.Range(minWalkTime, maxWalkTime);
                SetAnimatorState(true, false, walkAnimationSpeed);
                SetNewTargetPosition();
                break;

            case BoomboxState.Attacking:
                stateTimer = attackDuration;
                SetAnimatorState(false, true, attackAnimationSpeed);
                // Trigger attack animation
                if (animator.parameters.Length > 0)
                {
                    // Try to trigger attack if trigger parameter exists
                    foreach (var param in animator.parameters)
                    {
                        if (param.name == ATTACK_TRIGGER && param.type == AnimatorControllerParameterType.Trigger)
                        {
                            animator.SetTrigger(ATTACK_TRIGGER);
                            break;
                        }
                    }
                }
                break;
        }
    }

    void SetAnimatorState(bool isWalking, bool isAttacking, float animSpeed)
    {
        if (animator == null) return;

        // Set animation speed
        animator.SetFloat("Speed", animSpeed);

        // Set boolean parameters if they exist
        SetAnimatorBool(IS_WALKING_PARAM, isWalking);
        SetAnimatorBool(IS_ATTACKING_PARAM, isAttacking);
    }

    void SetAnimatorBool(string parameterName, bool value)
    {
        foreach (var param in animator.parameters)
        {
            if (param.name == parameterName && param.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(parameterName, value);
                return;
            }
        }
    }

    void UpdateAnimatorParameters()
    {
        if (animator == null) return;

        // Update speed parameter based on current horizontal velocity
        float currentSpeed = Mathf.Abs(rb.linearVelocity.x);

        // Set speed parameter if it exists
        foreach (var param in animator.parameters)
        {
            if (param.name == SPEED_PARAM && param.type == AnimatorControllerParameterType.Float)
            {
                animator.SetFloat(SPEED_PARAM, currentSpeed);
                break;
            }
        }
    }

    void SetNewTargetPosition()
    {
        Vector2 randomPoint = patrolArea.GetRandomPointInside();
        // Keep the target at the same Y level as current position for horizontal-only movement
        targetPosition = new Vector2(randomPoint.x, transform.position.y);
        isMovingToTarget = true;
    }

    void MoveTowardsTarget()
    {
        // Only calculate horizontal direction
        float direction = targetPosition.x - transform.position.x;

        // Only set horizontal velocity, preserve vertical velocity for gravity
        if (Mathf.Abs(direction) > 0.1f)
        {
            float horizontalVelocity = Mathf.Sign(direction) * walkSpeed;
            rb.linearVelocity = new Vector2(horizontalVelocity, rb.linearVelocity.y);

            // Flip sprite based on movement direction
            spriteRenderer.flipX = direction < 0;
        }
        else
        {
            // Stop horizontal movement when close to target
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void CheckAreaConstraints()
    {
        Vector2 currentPos = transform.position;

        if (!patrolArea.IsPointInside(currentPos))
        {
            Vector2 correctedPosition = patrolArea.GetNearestPointInside(currentPos);
            // Only correct X position, keep current Y position
            transform.position = new Vector2(correctedPosition.x, transform.position.y);

            if (currentState == BoomboxState.Walking)
            {
                SetNewTargetPosition();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (isMovingToTarget)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
        }
    }
}
