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
    readonly int runningHash = Animator.StringToHash("Running");
    readonly int jumpHash = Animator.StringToHash("Jump");
    readonly int deathHash = Animator.StringToHash("Death");
    readonly int victoryHash = Animator.StringToHash("Victory");


    void Start()
    {
        animator = GetComponent<Animator>();
        ResetRun();
    }

    void Update()
    {
        if (isDead || isVictorious) return;

        if (!hasInputStarted)
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.anyKey.wasPressedThisFrame)
            {
                BeginWalkingPhase(); // première entrée utilisateur : on lance la phase de marche
            }
        }
        else if (!hasStartedRunning)
        {
            walkTimer -= Time.deltaTime;
            if (walkTimer <= 0f)
            {
                StartRunning(); // après la marche, on passe en course
            }
        }

        if (isJumping)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
            {
                isJumping = false;
                StartRunning(); // retour automatique à la course après le saut
            }
        }
    }

    public void ResetRun()
    {
        isDead = false;
        hasInputStarted = false;
        hasStartedRunning = false;
        isWalking = false;
        isJumping = false;
        currentHealth = maxHealth;
        PlayState(idleHash);
    }

    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(currentHealth - amount, 0);
        if (currentHealth == 0)
        {
            HandleDeath();
        }
    }

    public bool TryStartJump()
    {
        if (isDead || isJumping) return false;

        StartJump();
        return true;
    }

    void StartRunning()
    {
        hasStartedRunning = true;
        isWalking = false;
        PlayState(runningHash); // déclenche l'anim de course
    }

    void StartJump()
    {
        isJumping = true;
        hasStartedRunning = true;
        isWalking = false;
        jumpTimer = jumpReturnDelay;
        PlayState(jumpHash); // déclenche l'anim de saut
    }

    void HandleDeath()
    {
        isDead = true;
        PlayState(deathHash); // déclenche l'anim de mort
    }


    public void TriggerVictory()
    {
        if (isDead || isJumping) return;
        HandleVictory();
    }

    void HandleVictory()
    {
        isVictorious = true;  
        PlayState(victoryHash);
        hasStartedRunning = false;
        isWalking = false;
        isJumping = false;
    }




    void PlayState(int stateHash)
    {
        Debug.Log("Playing state: " + stateHash); 

        if (animator == null) return;

        if (animator.HasState(0, stateHash))
        {
            animator.CrossFadeInFixedTime(stateHash, stateCrossFade);
        }
        else if (animator.HasState(0, idleHash))
        {
            animator.CrossFadeInFixedTime(idleHash, stateCrossFade);
        }
    }


    void BeginWalkingPhase()
    {
        hasInputStarted = true;
        hasStartedRunning = false;
        isWalking = true;
        walkTimer = walkLeadSeconds;
        PlayState(walkingHash);
    }
}
