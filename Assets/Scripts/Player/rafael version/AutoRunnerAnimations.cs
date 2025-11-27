using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class AutoRunnerAnimation : MonoBehaviour
{
    [Header("Auto Run Settings")]
    public float stateCrossFade = 0.2f;
    public bool autoStart = false;

    [Header("Jump Settings")]
    public float jumpReturnDelay = 0.8f;

    [Header("Health Settings")]
    public int maxHealth = 3;

    [Header("UI")]
    public HealthBarUI healthBarUI;

    Animator animator;
    int currentHealth;
    float jumpTimer;

    bool hasInputStarted;
    bool hasStartedRunning;
    bool isWalking;
    bool isJumping;
    bool isDead;
    bool isVictorious;
    bool isCrouching;

    public bool IsVictorious => isVictorious;
    public bool IsDead => isDead;
    public bool HasInputStarted => hasInputStarted;
    public bool HasStartedRunning => hasStartedRunning;
    public bool IsJumping => isJumping;
    public bool IsCrouching => isCrouching;

    readonly int idleHash = Animator.StringToHash("Idle");
    readonly int walkingHash = Animator.StringToHash("Walking");
    readonly int joggingHash = Animator.StringToHash("Jog Forward");
    readonly int runningHash = Animator.StringToHash("Running");
    readonly int jumpHash = Animator.StringToHash("Jump");
    readonly int crouchHash = Animator.StringToHash("Crouching Idle");
    readonly int deathHash = Animator.StringToHash("Death");
    readonly int victoryHash = Animator.StringToHash("Victory");

    private ItemsManager.SpeedStage lastStage = ItemsManager.SpeedStage.Walking;

    void Start()
    {
        animator = GetComponent<Animator>();
        ResetRun();

        if (healthBarUI != null)
        {
            healthBarUI.SetMaxHealth(maxHealth);
            healthBarUI.SetHealth(currentHealth);
        }

        if (autoStart)
            StartMovementPhase();
    }

    void Update()
    {
        if (isDead || isVictorious) return;

        var keyboard = Keyboard.current;

        if (!autoStart && !hasInputStarted && keyboard != null && keyboard.anyKey.wasPressedThisFrame)
            StartMovementPhase();

        if (hasStartedRunning && !isJumping && !isCrouching)
            UpdateMovementAnimation();

        if (isJumping)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
            {
                isJumping = false;
                if (isCrouching)
                    PlayState(crouchHash);
                else
                {
                    lastStage = ItemsManager.SpeedStage.Walking;
                    UpdateMovementAnimation();
                }
            }
        }
    }

    void UpdateMovementAnimation(bool forceUpdate = false)
    {
        if (isCrouching) return;

        ItemsManager.SpeedStage currentStage = ItemsManager.CurrentStage;

        if (forceUpdate || currentStage != lastStage || !hasStartedRunning)
        {
            lastStage = currentStage;

            switch (currentStage)
            {
                case ItemsManager.SpeedStage.Walking: PlayState(walkingHash); break;
                case ItemsManager.SpeedStage.Jogging: PlayState(joggingHash); break;
                case ItemsManager.SpeedStage.Running: PlayState(runningHash); break;
            }

            hasStartedRunning = true;
        }
    }

    public void TriggerCrouch()
    {
        if (isDead || isVictorious) return;
        isCrouching = true;
        PlayState(crouchHash);
    }

    public void ExitCrouch()
    {
        if (isDead || isVictorious) return;
        isCrouching = false;
        UpdateMovementAnimation(forceUpdate: true);
    }

    public void ResetRun()
    {
        ItemsManager.ResetCoins();

        isDead = false;
        hasInputStarted = false;
        hasStartedRunning = false;
        isWalking = false;
        isJumping = false;
        isCrouching = false;
        isVictorious = false;

        currentHealth = maxHealth;

        lastStage = ItemsManager.SpeedStage.Walking;

        PlayState(idleHash);

        if (healthBarUI != null)
        {
            healthBarUI.SetMaxHealth(maxHealth);
            healthBarUI.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);

        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth);

        if (currentHealth == 0)
            HandleDeath();
    }

    void HandleDeath()
    {
        isDead = true;
        isCrouching = false;
        PlayState(deathHash);

        LevelManager level = FindFirstObjectByType<LevelManager>();
        if (level != null)
            level.PlayerDied();
    }

    public bool TryStartJump()
    {
        if (isDead || isJumping || isCrouching) return false;

        StartJump();
        return true;
    }

    void StartJump()
    {
        isJumping = true;
        hasStartedRunning = true;
        isWalking = false;
        jumpTimer = jumpReturnDelay;

        PlayState(jumpHash);
    }

    public void TriggerVictory()
    {
        if (isDead || isJumping) return;
        HandleVictory();
    }

    void HandleVictory()
    {
        isVictorious = true;
        isCrouching = false;
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
            animator.CrossFadeInFixedTime(stateHash, stateCrossFade);
        else
            animator.CrossFadeInFixedTime(idleHash, stateCrossFade);
    }

    void StartMovementPhase()
    {
        hasInputStarted = true;
        isWalking = true;
        isCrouching = false;

        lastStage = ItemsManager.SpeedStage.Walking;
        UpdateMovementAnimation();
    }
}
