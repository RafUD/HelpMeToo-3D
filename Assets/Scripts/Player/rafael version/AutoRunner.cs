using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AutoRunnerAnimation))]
public class AutoRunner : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float jogSpeed = 10f;
    public float runSpeed = 15f;
    public float strafeSpeed = 7f;
    public float turnSpeed = 10f;
    public float tiltAngle = 5f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float gravity = -50f;

    CharacterController controller;
    AutoRunnerAnimation anim;

    float verticalVelocity;
    Vector3 baseForward;
    bool isCrouching = false;
    public bool IsCrouching => isCrouching;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<AutoRunnerAnimation>();
        baseForward = transform.forward.normalized;
    }

    void Update()
    {
        // Stop all movement if dead or victorious
        if (anim != null && (anim.IsDead || anim.IsVictorious))
        {
            verticalVelocity = 0f;
            isCrouching = false;
            return;
        }

        var keyboard = Keyboard.current;
        bool jumpPressed = keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
        bool crouchHeld = keyboard != null && keyboard.leftShiftKey.isPressed;

        // Handle crouch 
        if (crouchHeld)
        {
            if (!isCrouching)
            {
                EnterCrouch();
            }
        }
        else
        {
            if (isCrouching)
            {
                ExitCrouch();
            }
        }

        // Horizontal input for strafing
        float horizontal = 0f;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed) horizontal -= 1f;
            if (keyboard.dKey.isPressed) horizontal += 1f;
        }

        bool canMove = anim != null && anim.HasInputStarted;

        // CROUCH == no forward movement, no jump
        if (isCrouching)
        {
            if (controller.isGrounded)
                verticalVelocity = -2f;
        }
        else
        {
            // Handle jump
            if (jumpPressed && controller.isGrounded && canMove)
            {
                if (anim.TryStartJump())
                {
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }

            // Apply gravity
            verticalVelocity += gravity * Time.deltaTime;
            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
        }

        // Speed based on coin stage
        float forwardSpeed = (canMove && !isCrouching) ? GetCurrentSpeed() : 0f;

        // Lateral movement
        float lateralDelta = 0f;
        if (canMove && !isCrouching) // block lateral movement when crouched
        {
            float targetZ = Mathf.Clamp(transform.position.z + horizontal * strafeSpeed * Time.deltaTime,
                                        LevelBoundary.leftSide, LevelBoundary.rightSide);
            lateralDelta = targetZ - transform.position.z;
        }


        // Rotation (tilt)
        float targetYaw = canMove ? Mathf.Clamp(horizontal, -1f, 1f) * tiltAngle : 0f;
        Quaternion targetRot = Quaternion.AngleAxis(targetYaw, Vector3.up) * Quaternion.LookRotation(baseForward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        // Move character
        Vector3 moveVector = (baseForward * forwardSpeed + new Vector3(0f, 0f, lateralDelta / Time.deltaTime)) * Time.deltaTime;
        moveVector.y = verticalVelocity * Time.deltaTime;
        controller.Move(moveVector);
    }

    void EnterCrouch()
    {
        isCrouching = true;
        anim?.TriggerCrouch();
    }

    void ExitCrouch()
    {
        isCrouching = false;
        anim?.ExitCrouch();
    }

    float GetCurrentSpeed()
    {
        switch (ItemsManager.CurrentStage)
        {
            case ItemsManager.SpeedStage.Walking: return walkSpeed;
            case ItemsManager.SpeedStage.Jogging: return jogSpeed;
            case ItemsManager.SpeedStage.Running: return runSpeed;
            default: return walkSpeed;
        }
    }
}
