using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController2D controller;
    public float runSpeed = 40f;

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;
    private PlayerStats playerStats;
    private Animator animator;
    private Rigidbody2D rb2D;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (playerStats != null && playerStats.IsAlive())
        {
            HandleInput();
            UpdateJumpAnimation();
        }
    }

    void HandleInput()
    {
        if (Keyboard.current != null)
        {
            float horizontal = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontal = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontal = 1f;

            // Adjust horizontal movement based on grounded state
            if (controller.getGrounder())
                horizontalMove = horizontal * runSpeed;
            else
                horizontalMove = horizontal * runSpeed * 0.5f; // Reduced air control

            if (animator != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(horizontal));
            }

            // Allow jump only if grounded
            if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.getGrounder())
            {
                jump = true;
            }

            if (Mouse.current != null)
            {
                // Button was pressed this frame
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    animator.SetBool("isAttacking", true);
                }
            }

            crouch = Keyboard.current.sKey.isPressed || Keyboard.current.leftCtrlKey.isPressed;
        }
    }

    void UpdateJumpAnimation()
    {
        if (animator == null || rb2D == null) return;

        float velocityY = rb2D.linearVelocity.y;
        animator.SetFloat("yVelocity", velocityY);

        bool isGrounded = controller.getGrounder();

        // Don't immediately switch to idle just because grounded if vertical velocity is not near 0
        if (isGrounded && Mathf.Abs(velocityY) < 0.1f)
        {
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetBool("isJumping", true);
        }
    }

    void FixedUpdate()
    {
        if (playerStats != null && playerStats.IsAlive())
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false; // Reset jump after being applied
        }
    }
}
