using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController2D controller;
    public float runSpeed = 40f;

    // Sound
    public AudioSource walkingSound;
    
    private float horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerMovement requires PlayerStats component!");
        }
    }

    void Update()
    {
        // Only allow movement if player is alive
        if (playerStats != null && playerStats.IsAlive())
        {
            HandleInput();
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

            horizontalMove = horizontal * runSpeed;

            // Jump input
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                jump = true;

            // Crouch input
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

    void FixedUpdate()
    {
        // Only move if player is alive
        if (playerStats != null && playerStats.IsAlive())
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }
    }
}
