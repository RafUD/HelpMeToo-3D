using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(animationStateController))]
public class RunnerMover : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 10f;
    public float strafeSpeed = 7f;
    public float lateralLimit = 3f;

    [Header("Jump")]
    public float jumpHeight = 1.5f;
    public float gravity = -25f;
    public float groundedGraceTime = 0.1f;

    CharacterController controller;
    animationStateController anim;
    float verticalVelocity;
    float coyoteTimer;
    float lateralOffset;
    Vector3 startPosition;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<animationStateController>();
        startPosition = transform.position;
        lateralOffset = 0f;
    }

    void Update()
    {
        if (anim != null && anim.IsDead)
        {
            verticalVelocity = 0f;
            return;
        }

        bool grounded = controller.isGrounded;
        if (grounded)
        {
            coyoteTimer = groundedGraceTime;
            if (verticalVelocity < 0f) verticalVelocity = -2f; // keep stuck to ground
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
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

        if (jumpPressed && coyoteTimer > 0f && canMove)
        {
            bool startedJump = anim == null || anim.TryStartJump();
            if (startedJump)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                coyoteTimer = 0f;
            }
        }

        float forwardSpeed = 0f;
        if (canMove)
        {
            forwardSpeed = (anim != null && anim.HasStartedRunning) ? runSpeed : walkSpeed;
        }
        float targetLateralOffset = lateralOffset;
        float lateralDeltaPerSec = 0f;
        if (canMove)
        {
            targetLateralOffset = Mathf.Clamp(lateralOffset + horizontal * strafeSpeed * Time.deltaTime, -lateralLimit, lateralLimit);
            float lateralDelta = targetLateralOffset - lateralOffset;
            lateralDeltaPerSec = (Mathf.Abs(lateralDelta) > Mathf.Epsilon) ? (lateralDelta / Time.deltaTime) : 0f;
        }

        Vector3 move = (transform.forward * forwardSpeed) + (transform.right * lateralDeltaPerSec);
        controller.Move(move * Time.deltaTime);
        lateralOffset = targetLateralOffset;

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }
}
