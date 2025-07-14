using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController2D controller;
    public float runSpeed = 40f;

    // Sound
    public AudioSource walkingSound;
    
    [Header("Effects")]
    public GameObject runEffectObject; // Drag your RunEffect GameObject here in Inspector
    public GameObject jumpEffectObject; // Drag your JumpEffect GameObject here in Inspector

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;
    private PlayerStats playerStats;
    private Animator animator;
    private Rigidbody2D rb2D;

    // For landing detection
    private bool wasGrounded = true;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();

        // Find RunEffect if not assigned in Inspector
        if (runEffectObject == null)
        {
            Transform runEffectTransform = transform.Find("RunEffect");
            if (runEffectTransform != null)
            {
                runEffectObject = runEffectTransform.gameObject;
                runEffectObject.SetActive(false); // Start hidden
            }
        }

        // Find JumpEffect if not assigned in Inspector
        if (jumpEffectObject == null)
        {
            Transform jumpEffectTransform = transform.Find("JumpEffect");
            if (jumpEffectTransform != null)
            {
                jumpEffectObject = jumpEffectTransform.gameObject;
                jumpEffectObject.SetActive(false); // Start hidden
            }
        }

        // Initialize grounded state
        wasGrounded = controller.getGrounder();
    }

    void Update()
    {
        if (playerStats != null && playerStats.IsAlive())
        {
            HandleInput();
            UpdateJumpAnimation();
            UpdateRunEffect();
            UpdateJumpEffect();
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

            if (controller.getGrounder())
                horizontalMove = horizontal * runSpeed;
            else
                horizontalMove = horizontal * runSpeed * 0.5f;

            if (animator != null)
            {
                animator.SetFloat("Speed", Mathf.Abs(horizontal));
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.getGrounder())
            {
                jump = true;
            }

            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    animator.SetBool("isAttacking", true);
                }
            }

            crouch = Keyboard.current.sKey.isPressed || Keyboard.current.leftCtrlKey.isPressed;
        }

        playSound();
    }

    private void playSound()
    {
        bool playerIsWalking = Keyboard.current.aKey.isPressed || Keyboard.current.dKey.isPressed;
        if (controller.IsGrounded() && playerIsWalking)
        {
            walkingSound.enabled = true;
        }
        else
        {
            walkingSound.enabled = false;
        }
    }

    void UpdateRunEffect()
    {
        if (runEffectObject == null) return;

        bool isGrounded = controller.getGrounder();
        bool isMoving = Mathf.Abs(horizontalMove) > 0.1f;
        bool shouldShowEffect = isGrounded && isMoving;

        if (runEffectObject.activeInHierarchy != shouldShowEffect)
        {
            runEffectObject.SetActive(shouldShowEffect);
        }
    }

    void UpdateJumpEffect()
    {
        if (jumpEffectObject == null) return;

        bool isGrounded = controller.getGrounder();

        // Check if character just landed (was not grounded, now grounded)
        if (!wasGrounded && isGrounded)
        {
            // Position the jump effect at the character's current landing position
            PositionJumpEffectAtLandingSpot();

            // Play the landing effect
            jumpEffectObject.SetActive(true);

            // Automatically hide the effect after animation completes
            StartCoroutine(HideJumpEffectAfterTime(0.6f)); // Slightly longer than your 0.58s animation
        }

        // Update previous grounded state
        wasGrounded = isGrounded;
    }

    void PositionJumpEffectAtLandingSpot()
    {
        // Get the character's current position
        Vector3 characterPosition = transform.position;

        // Position the jump effect at the character's feet/base
        // Adjust the Y offset to place it at ground level (beneath the character)
        Vector3 landingPosition = new Vector3(
            characterPosition.x,                    // Same X as character
            characterPosition.y - 0.7f,          // Slightly below character (adjust as needed)
            characterPosition.z                     // Same Z as character
        );

        // Move the jump effect to the landing position
        jumpEffectObject.transform.position = landingPosition;
    }

    System.Collections.IEnumerator HideJumpEffectAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (jumpEffectObject != null)
        {
            jumpEffectObject.SetActive(false);
        }
    }

    void UpdateJumpAnimation()
    {
        if (animator == null || rb2D == null) return;

        float velocityY = rb2D.linearVelocity.y;
        animator.SetFloat("yVelocity", velocityY);

        bool isGrounded = controller.getGrounder();

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
            jump = false;
        }
    }
}
