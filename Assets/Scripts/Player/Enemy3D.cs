using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyNavMeshAI : MonoBehaviour
{
    [Header("References")]
    public Transform[] patrolPoints;
    public Transform player;

    [Header("Settings")]
    public float detectionRange = 10f;
    public float patrolSpeed = 3f;
    public float rotationSpeed = 5f;
    public float attackRange = 1.5f;

    [Header("Player Speed Matching")]
    public float speedOffset = 1.0f; // How much faster than the player
    public bool alwaysMatchPlayerSpeed = false;

    private NavMeshAgent agent;
    private Animator animator;
    private int patrolIndex = 0;
    private int currentAnimHash = -1;

    private readonly int walkHash = Animator.StringToHash("Walk");
    private readonly int runHash = Animator.StringToHash("Running");
    private readonly int attackHash = Animator.StringToHash("Attack");

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

        PlayAnimation(walkHash);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Get player speed
        float playerSpeed = player.GetComponent<AutoRunner>().GetCurrentSpeed();

        // Always match player speed + offset
        if (alwaysMatchPlayerSpeed)
        {
            agent.speed = playerSpeed + speedOffset;
        }

        if (distanceToPlayer <= detectionRange)
        {
            // Chase player
            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                PlayAnimation(attackHash);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                PlayAnimation(runHash);
            }
        }
        else
        {
            // Patrol behavior
            agent.speed = patrolSpeed;
            if (!agent.pathPending && agent.remainingDistance < 0.2f && patrolPoints.Length > 0)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[patrolIndex].position);
            }
            PlayAnimation(walkHash);
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerAnim = other.GetComponent<AutoRunnerAnimation>();
        if (playerAnim != null && !playerAnim.IsDead)
        {
            PlayAnimation(attackHash);
            playerAnim.TakeDamage(playerAnim.maxHealth);

            var level = FindFirstObjectByType<LevelManager>();
            if (level != null) level.PlayerDied();
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
