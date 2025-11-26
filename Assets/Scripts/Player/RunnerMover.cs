using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(animationStateController))]
public class RunnerMover : MonoBehaviour
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
    public float groundedGraceTime = 0.1f;

    CharacterController controller;
    animationStateController anim;
    float verticalVelocity;
    float coyoteTimer;
    Vector3 baseForward;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<animationStateController>();
        baseForward = transform.forward.normalized;
    }

    void Update()
    {
        if (anim != null && (anim.IsDead || anim.IsVictorious))
        {
            verticalVelocity = 0f;
            return;
        }

        var keyboard = Keyboard.current;
        bool jumpPressed = keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
        float horizontal = 0f;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed) horizontal -= 1f;
            if (keyboard.dKey.isPressed) horizontal += 1f;
        }

        bool canMove = anim == null || anim.HasInputStarted;

        // Handle jump input FIRST
        if (jumpPressed && controller.isGrounded && canMove)
        {
            bool startedJump = anim == null || anim.TryStartJump();
            if (startedJump)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                Debug.Log("JUMP! Velocity: " + verticalVelocity);
            }
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Apply downward force when grounded
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        Debug.Log($"Grounded: {controller.isGrounded} | Velocity: {verticalVelocity:F2} | Y: {transform.position.y:F2}");

        // Horizontal movement
        float forwardSpeed = canMove ? GetCurrentSpeed() : 0f;

        float targetZ = transform.position.z;
        float lateralDeltaPerSec = 0f;
        if (canMove)
        {
            targetZ = Mathf.Clamp(transform.position.z + horizontal * strafeSpeed * Time.deltaTime,
                                  LevelBoundary.leftSide, LevelBoundary.rightSide);
            float lateralDelta = targetZ - transform.position.z;
            lateralDeltaPerSec = (Mathf.Abs(lateralDelta) > Mathf.Epsilon) ? (lateralDelta / Time.deltaTime) : 0f;
        }

        // Rotation
        float targetYaw = canMove ? Mathf.Clamp(horizontal, -1f, 1f) * tiltAngle : 0f;
        Quaternion targetRot = Quaternion.AngleAxis(targetYaw, Vector3.up) * Quaternion.LookRotation(baseForward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        // MOVE: Combine horizontal + vertical in ONE move
        Vector3 moveVector = (baseForward * forwardSpeed + new Vector3(0f, 0f, lateralDeltaPerSec)) * Time.deltaTime;
        moveVector.y = verticalVelocity * Time.deltaTime;

        controller.Move(moveVector);
    }

    float GetCurrentSpeed()
    {
        switch (ItemsManager.CurrentStage)
        {
            case ItemsManager.SpeedStage.Walking:
                return walkSpeed;
            case ItemsManager.SpeedStage.Jogging:
                return jogSpeed;
            case ItemsManager.SpeedStage.Running:
                return runSpeed;
            default:
                return walkSpeed;
        }
    }
}