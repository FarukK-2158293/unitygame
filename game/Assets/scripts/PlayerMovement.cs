using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;

    private float horizontalMove = 0f;
    private bool jump = false;
    private bool crouch = false;

    void Update()
    {
        // Get movement input
        if (Keyboard.current != null)
        {
            float horizontal = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontal = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontal = 1f;

            horizontalMove = horizontal * runSpeed;

            // Debug line - remove this after testing
            if (horizontal != 0)
                Debug.Log($"Input detected: {horizontal}, horizontalMove: {horizontalMove}");

            // Jump input
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                jump = true;

            // Crouch input
            crouch = Keyboard.current.sKey.isPressed || Keyboard.current.leftCtrlKey.isPressed;
        }
    }


    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}
