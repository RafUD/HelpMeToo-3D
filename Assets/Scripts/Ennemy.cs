using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform target; // Player

    [Header("Chase Settings")]
    public float detectionRange = 10f;
    public float chaseSpeed = 5f;

    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 3f;
    public float patrolWaitTime = 1f;

    private Rigidbody rb;
    private Animator animator;

    private Transform currentPatrolTarget;
    private bool isChasing;
    private bool isWaiting;
    private float patrolWaitTimer;
    private bool isPatrolling;

    // Track current animation
    private int currentAnimHash = -1;

    // Animator hashes
    private readonly int walkHash = Animator.StringToHash("Old Man Walk");
    private readonly int runHash = Animator.StringToHash("Running");
    private readonly int attackHash = Animator.StringToHash("Attack");

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentPatrolTarget = pointA;
        isPatrolling = false;

        PlayAnimation(walkHash); // Start patrol animation
    }

    void FixedUpdate()
    {
        if (LevelManager.GlobalFreeze) return;

        float distanceToPlayer = Vector3.Distance(target.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing) EnterChaseMode();
            ChasePlayer();
        }
        else
        {
            if (isChasing) ExitChaseMode();
            Patrol();
        }
    }

    void EnterChaseMode()
    {
        isChasing = true;
        isWaiting = false;
        isPatrolling = false;
        PlayAnimation(runHash);
    }

    void ExitChaseMode()
    {
        isChasing = false;
        isPatrolling = false;
        PlayAnimation(walkHash);
    }

    void Patrol()
    {
        if (!isPatrolling)
        {
            PlayAnimation(walkHash);
            isPatrolling = true;
        }

        if (isWaiting)
        {
            patrolWaitTimer -= Time.fixedDeltaTime;
            if (patrolWaitTimer <= 0f)
            {
                isWaiting = false;
                currentPatrolTarget = (currentPatrolTarget == pointA) ? pointB : pointA;
            }
            return;
        }

        Vector3 targetPos = new Vector3(currentPatrolTarget.position.x, transform.position.y, currentPatrolTarget.position.z);
        Vector3 direction = (targetPos - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
        }

        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, patrolSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector3.Distance(transform.position, targetPos) < 0.3f)
        {
            isWaiting = true;
            patrolWaitTimer = patrolWaitTime;
        }
    }

    void ChasePlayer()
    {
        PlayAnimation(runHash);

        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        Vector3 direction = (targetPos - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 8f);
        }

        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !LevelManager.GlobalFreeze)
        {
            PlayAnimation(attackHash);

            var anim = other.GetComponent<AutoRunnerAnimation>();
            if (anim != null) anim.TakeDamage(999);

            LevelManager.GlobalFreeze = true;
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager != null) levelManager.PlayerDied();
        }
    }

    private void PlayAnimation(int hash)
    {
        if (animator == null) return;

        if (currentAnimHash == hash) return;

        if (animator.HasState(0, hash))
        {
            // Play first frame immediately, then crossfade
            animator.Play(hash, 0, 0f);
            animator.CrossFadeInFixedTime(hash, 0.2f);
            currentAnimHash = hash;
        }
        else
        {
            Debug.LogWarning($"[Enemy] Animator state not found! Hash: {hash}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Patrol points
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.position, 0.5f);
        }
    }
}
