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
    public float turnSpeed = 10f;
    public float tiltAngle = 5f; // yaw visuel max en degres quand on strafe

    [Header("Jump")]
    public float jumpHeight = 1.5f;
    public float gravity = -25f;
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
        if (anim != null && anim.IsDead)
        {
            verticalVelocity = 0f; // mort : on ne bouge plus
            return;
        }

        bool grounded = controller.isGrounded;
        if (grounded)
        {
            coyoteTimer = groundedGraceTime;
            if (verticalVelocity < 0f) verticalVelocity = -2f; // garde le perso colle au sol
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
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity); // impulsion vers le haut
                coyoteTimer = 0f;
            }
        }

        float forwardSpeed = 0f;
        if (canMove)
        {
            forwardSpeed = (anim != null && anim.HasStartedRunning) ? runSpeed : walkSpeed;
        }

        // Limite gauche/droite basee sur LevelBoundary (axes Z)
        float targetZ = transform.position.z;
        float lateralDeltaPerSec = 0f;
        if (canMove)
        {
            targetZ = Mathf.Clamp(transform.position.z + horizontal * strafeSpeed * Time.deltaTime, LevelBoundary.leftSide, LevelBoundary.rightSide);
            float lateralDelta = targetZ - transform.position.z;
            lateralDeltaPerSec = (Mathf.Abs(lateralDelta) > Mathf.Epsilon) ? (lateralDelta / Time.deltaTime) : 0f;
        }

        Vector3 move = (baseForward * forwardSpeed) + new Vector3(0f, 0f, lateralDeltaPerSec);

        // Rotation legere pour donner l'impression de pencher vers la gauche/droite sans devier la trajectoire
        float targetYaw = 0f;
        if (canMove)
        {
            targetYaw = Mathf.Clamp(horizontal, -1f, 1f) * tiltAngle;
        }
        Quaternion targetRot = Quaternion.AngleAxis(targetYaw, Vector3.up) * Quaternion.LookRotation(baseForward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        controller.Move(move * Time.deltaTime);

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }
}
