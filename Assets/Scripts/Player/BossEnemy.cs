using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))] 
[RequireComponent(typeof(Animator))]     
public class BossNavMeshAI : MonoBehaviour
{
    [Header("References")]
    public Transform[] patrolPoints; 
    public Transform player;         

    [Header("Settings")]
    public float detectionRange = 10f;  
    public float chaseSpeed = 5f;       
    public float patrolSpeed = 3f;      
    public float rotationSpeed = 5f;    // How quickly boss rotates to face movement direction
    public int coinsRequiredToDefeat = 50; 
    public float attackRange = 2f;      // Distance at which boss attacks player

    private NavMeshAgent agent;   
    private Animator animator;    
    private int patrolIndex = 0;  // Current patrol target index
    private int currentAnimHash = -1; // Tracks which animation is currently playing

    // Hashes for animations for efficiency
    private readonly int runHash = Animator.StringToHash("Mutant Run");
    private readonly int attackWinHash = Animator.StringToHash("Mutant Punch");
    private readonly int attackLoseHash = Animator.StringToHash("Mutant Dying");
    private readonly int taunt = Animator.StringToHash("Standing Taunt Battlecry");

    private bool isTaunting = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.speed = patrolSpeed;
        agent.isStopped = true; // IMPORTANT: stop movement during taunt

        PlayAnimation(taunt);

        StartCoroutine(TauntThenStart());
    }


    private IEnumerator TauntThenStart()
    {
        // Adjust this to your taunt animation length
        yield return new WaitForSeconds(2.5f);

        isTaunting = false;
        agent.isStopped = false;

        if (patrolPoints.Length > 0)
        {
            patrolIndex = 0;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        PlayAnimation(runHash);
    }


    void Update()
    {

        if (isTaunting) return;


        if (player == null) return; // Skip if player is missing

        int playerCoins = ItemsManager.coinsCollected;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Player detected, switch to chase speed
            agent.speed = chaseSpeed;

            if (distanceToPlayer <= attackRange)
            {
                // Within attack range
                agent.isStopped = true;

                var playerAnim = player.GetComponent<AutoRunnerAnimation>();
                if (playerAnim != null)
                {
                    // Determine if player loses or boss loses based on coins
                    if (playerCoins >= coinsRequiredToDefeat)
                    {
                        // Player has enough coins to defeat boss
                        PlayAnimation(attackLoseHash);
                        FindFirstObjectByType<LevelManager>()?.WinLevel();
                        Destroy(gameObject, 2f); // Remove boss after animation
                    }
                    else
                    {
                        // Boss wins, player dies
                        PlayAnimation(attackWinHash);
                        playerAnim.TakeDamage(playerAnim.maxHealth);
                        FindFirstObjectByType<LevelManager>()?.PlayerDied();
                    }
                }
            }
            else
            {
                // Player detected but not in attack range
                agent.isStopped = false;
                if (agent.destination != player.position)
                    agent.SetDestination(player.position);

                PlayAnimation(runHash); // Run animation while chasing
            }
        }
        else
        {
            // Player not detected, patrol
            agent.speed = patrolSpeed;
            if (!agent.pathPending && agent.remainingDistance < 0.3f && patrolPoints.Length > 0)
            {
                // Move to next patrol point
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }

            PlayAnimation(runHash); // Run animation while patrolling
        }

        RotateSmooth(); // Rotate boss to face movement direction
    }

    private void RotateSmooth()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f; // Keep rotation on horizontal plane only
        if (velocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void PlayAnimation(int hash)
    {
        if (animator == null || currentAnimHash == hash) return; // Skip if already playing

        if (animator.HasState(0, hash))
        {
            animator.CrossFade(hash, 0.1f); // Smooth transition to new animation
            currentAnimHash = hash;
        }
    }

   
}
