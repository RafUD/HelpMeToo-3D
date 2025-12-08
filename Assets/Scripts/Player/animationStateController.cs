using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class animationStateController : MonoBehaviour
{
    [Header("Auto Run")]
    public float walkLeadSeconds = 1f;
    public float stateCrossFade = 0.1f;

    [Header("Jump")]
    public float jumpReturnDelay = 0.8f;

    [Header("Health")]
    public int maxHealth = 3;

    Animator animator;
    int currentHealth;
    float walkTimer;
    float jumpTimer;
    bool hasInputStarted;
    bool hasStartedRunning;
    bool isWalking;
    bool isJumping;
    bool isDead;
    bool isVictorious;

    public bool IsVictorious => isVictorious;
    public bool IsDead => isDead;
    public bool HasInputStarted => hasInputStarted;
    public bool IsWalking => isWalking;
    public bool HasStartedRunning => hasStartedRunning;
    public bool IsJumping => isJumping;

    readonly int idleHash = Animator.StringToHash("Idle");
    readonly int walkingHash = Animator.StringToHash("Walking");
    readonly int joggingHash = Animator.StringToHash("Jog Forward");
    readonly int runningHash = Animator.StringToHash("Running");
    readonly int jumpHash = Animator.StringToHash("Jump");
    readonly int deathHash = Animator.StringToHash("Death");
    readonly int victoryHash = Animator.StringToHash("Victory");

    private ItemsManager.SpeedStage lastStage = ItemsManager.SpeedStage.Walking;

    void Start()
    {
        animator = GetComponent<Animator>();
        ResetRun();
    }

    void Update()
    {
        if (isDead || isVictorious) return;

        // ENTRY
        if (!hasInputStarted)
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.anyKey.wasPressedThisFrame)
            {
                Debug.Log("[STATE] Input detected -> BeginWalkingPhase()");
                BeginWalkingPhase();
            }
        }
        else if (!hasStartedRunning)
        {
            walkTimer -= Time.deltaTime;
            if (walkTimer <= 0f)
            {
                Debug.Log("[STATE] walkTimer finished -> StartRunning()");
                StartRunning();
            }
        }

        // Movement animation switching
        if (hasStartedRunning && !isJumping)
        {
            UpdateMovementAnimation();
        }

        if (isJumping)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
            {
                Debug.Log("[JUMP] Returned from jump. Forcing state update.");
                isJumping = false;
                StartRunning();
                UpdateMovementAnimation();
            }
        }
    }

    void UpdateMovementAnimation()
    {
        ItemsManager.SpeedStage currentStage = ItemsManager.CurrentStage;

        // Print coin count + stage
        Debug.Log($"[SPEED] Coins: {ItemsManager.coinsCollected} | Stage: {currentStage}");

        if (currentStage != lastStage)
        {
            Debug.Log($"[SPEED] Stage changed: {lastStage} -> {currentStage}");
            lastStage = currentStage;

            switch (currentStage)
            {
                case ItemsManager.SpeedStage.Walking:
                    Debug.Log("[ANIM] Playing WALKING");
                    PlayState(walkingHash);
                    break;

                case ItemsManager.SpeedStage.Jogging:
                    Debug.Log("[ANIM] Playing JOG FORWARD");
                    PlayState(joggingHash);
                    break;

                case ItemsManager.SpeedStage.Running:
                    Debug.Log("[ANIM] Playing RUNNING");
                    PlayState(runningHash);
                    break;
            }
        }
    }

    public void ResetRun()
    {
        Debug.Log("[RESET] ResetRun() called.");

        // **RESET COINS**
        ItemsManager.coinsCollected = 0;
        ItemsManager.CurrentStage = ItemsManager.SpeedStage.Walking;

        Debug.Log("[RESET] Coins reset to 0. Stage reset to WALKING.");

        isDead = false;
        hasInputStarted = false;
        hasStartedRunning = false;
        isWalking = false;
        isJumping = false;
        currentHealth = maxHealth;
        lastStage = ItemsManager.SpeedStage.Walking;
        PlayState(idleHash);

        Debug.Log("[RESET] Player health, movement flags and animation reset.");
    }

    public void TakeDamage(int amount = 1)
    {
        Debug.Log($"[DAMAGE] Player takes {amount} damage. Health BEFORE: {currentHealth}");

        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log($"[DAMAGE] Health AFTER: {currentHealth}");

        if (currentHealth == 0)
        {
            Debug.Log("[DAMAGE] Health reached 0 -> HandleDeath()");
            HandleDeath();
        }
    }

    public bool TryStartJump()
    {
        if (isDead || isJumping) return false;

        Debug.Log("[JUMP] TryStartJump() -> StartJump()");
        StartJump();
        return true;
    }

    void StartRunning()
    {
        hasStartedRunning = true;
        isWalking = false;

        Debug.Log("[STATE] StartRunning() -> Updating movement animation");
        UpdateMovementAnimation();
    }

    void StartJump()
    {
        isJumping = true;
        hasStartedRunning = true;
        isWalking = false;
        jumpTimer = jumpReturnDelay;

        Debug.Log("[JUMP] Playing jump animation.");
        PlayState(jumpHash);
    }

    void HandleDeath()
    {
        Debug.Log("[DEATH] Playing death animation.");
        isDead = true;
        PlayState(deathHash);
    }

    public void TriggerVictory()
    {
        if (isDead || isJumping) return;
        Debug.Log("[VICTORY] TriggerVictory() called");
        HandleVictory();
    }

    void HandleVictory()
    {
        Debug.Log("[VICTORY] Playing victory animation.");
        isVictorious = true;
        PlayState(victoryHash);
        hasStartedRunning = false;
        isWalking = false;
        isJumping = false;
    }

    void PlayState(int stateHash)
    {
        if (animator == null)
        {
            Debug.LogError("[ANIM] Animator missing!");
            return;
        }

        if (animator.HasState(0, stateHash))
        {
            Debug.Log($"[ANIM] CrossFade to state hash {stateHash}");
            animator.CrossFadeInFixedTime(stateHash, stateCrossFade);
        }
        else
        {
            Debug.LogWarning($"[ANIM] Missing state hash {stateHash}! Falling back to IDLE.");
            animator.CrossFadeInFixedTime(idleHash, stateCrossFade);
        }
    }

    void BeginWalkingPhase()
    {
        Debug.Log("[STATE] BeginWalkingPhase() - Starting walking phase.");
        hasInputStarted = true;
        hasStartedRunning = false;
        isWalking = true;
        walkTimer = walkLeadSeconds;

        PlayState(walkingHash);
    }
}

