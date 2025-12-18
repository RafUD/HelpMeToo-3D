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

    [Header("Combat")]
    public int coinsRequiredToDefeat = 10;

    [Header("Player Speed Matching")]
    public float speedOffset = 1.0f;
    public bool alwaysMatchPlayerSpeed = false;

    private NavMeshAgent agent;
    private Animator animator;
    private int patrolIndex = 0;
    private int currentAnimHash = -1;
    private bool isDead = false;

    // Animation hashes
    private readonly int walkHash = Animator.StringToHash("Walk");
    private readonly int runHash = Animator.StringToHash("Running");
    private readonly int attackHash = Animator.StringToHash("Attack");
    private readonly int deathHash = Animator.StringToHash("Zombie Death");



    bool godModeTargeting = false;


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
        if (player == null || isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Match player speed if enabled
        if (alwaysMatchPlayerSpeed)
        {
            var runner = player.GetComponent<AutoRunner>();
            if (runner != null)
                agent.speed = runner.GetCurrentSpeed() + speedOffset;
        }

        if (distanceToPlayer <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            PlayAnimation(runHash);
        }
        else
        {
            Patrol();
            PlayAnimation(walkHash);
        }

        RotateSmooth();



    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.2f && patrolPoints.Length > 0)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void RotateSmooth()
    {
        Vector3 velocity = agent.velocity;
        velocity.y = 0f;

        if (velocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {


        if (isDead || !other.CompareTag("Player")) return;

        int playerCoins = ItemsManager.coinsCollected;
        var playerAnim = other.GetComponent<AutoRunnerAnimation>();

        if (playerAnim == null || playerAnim.IsDead) return;

        if (playerCoins >= coinsRequiredToDefeat)
        {
            // Player defeats enemy
            Die();
        }
        else
        {
            // Enemy kills player
            PlayAnimation(attackHash);
            playerAnim.TakeDamage(playerAnim.maxHealth);

            FindFirstObjectByType<LevelManager>()?.PlayerDied();
        }
    }

    private void Die()
    {
        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        PlayAnimation(deathHash);

        Destroy(gameObject, 2.5f); // let animation finish
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


    public void EnterGodModeTarget()
    {
        godModeTargeting = true;
        agent.isStopped = false;
    }

}
