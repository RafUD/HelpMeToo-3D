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
    public float rotationSpeed = 5f;
    public int coinsRequiredToDefeat = 50;
    public float attackRange = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private int patrolIndex = 0;
    private int currentAnimHash = -1;

    private readonly int runHash = Animator.StringToHash("Mutant Run");
    private readonly int attackWinHash = Animator.StringToHash("Mutant Punch"); 
    private readonly int attackLoseHash = Animator.StringToHash("Mutant Dying");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
        {
            patrolIndex = 0;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        PlayAnimation(runHash);
    }

    void Update()
    {
        if (player == null) return;

        int playerCoins = ItemsManager.coinsCollected;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            agent.speed = chaseSpeed;

            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;

                var playerAnim = player.GetComponent<AutoRunnerAnimation>();
                if (playerAnim != null)
                {
                    if (playerCoins >= coinsRequiredToDefeat)
                    {
                        PlayAnimation(attackLoseHash);
                        FindFirstObjectByType<LevelManager>()?.WinLevel();
                        Destroy(gameObject, 2f);
                    }
                    else
                    {
                        PlayAnimation(attackWinHash);
                        playerAnim.TakeDamage(playerAnim.maxHealth);
                        FindFirstObjectByType<LevelManager>()?.PlayerDied();
                    }
                }
            }
            else
            {
                agent.isStopped = false;
                if (agent.destination != player.position)
                    agent.SetDestination(player.position);

                PlayAnimation(runHash);
            }
        }
        else
        {
            agent.speed = patrolSpeed;
            if (!agent.pathPending && agent.remainingDistance < 0.3f && patrolPoints.Length > 0)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }

            PlayAnimation(runHash);
        }

        RotateSmooth();
    }

    private void RotateSmooth()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f;
        if (velocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    private void PlayAnimation(int hash)
    {
        if (animator == null || currentAnimHash == hash) return;
        if (animator.HasState(0, hash))
        {
            animator.CrossFade(hash, 0.1f);
            currentAnimHash = hash;
        }
    }
}
