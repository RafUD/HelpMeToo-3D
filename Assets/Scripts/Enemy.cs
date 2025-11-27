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
    private float patrolWaitTimer;

    private int currentAnimHash = -1;

    private readonly int walkHash = Animator.StringToHash("Old Man Walk");
    private readonly int runHash = Animator.StringToHash("Running");
    private readonly int attackHash = Animator.StringToHash("Attack");




    void Start()
    {



        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentPatrolTarget = pointB;
        isChasing = false;
        patrolWaitTimer = 0f;

        PlayAnimation(walkHash);
    }

    void Update()
    {
        var playerAnim = target.GetComponent<AutoRunnerAnimation>();
        if (playerAnim != null && playerAnim.IsDead) return;

        float distanceToPlayer = Vector3.Distance(target.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing)
            {
                isChasing = true;
                patrolWaitTimer = 0f;
            }
            ChasePlayer();
            PlayAnimation(runHash);
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
            }
            Patrol();
            PlayAnimation(walkHash);
        }
    }

    void Patrol()
    {
        if (patrolWaitTimer > 0f)
        {
            patrolWaitTimer -= Time.fixedDeltaTime;
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

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentPatrolTarget = (currentPatrolTarget == pointA) ? pointB : pointA;
            patrolWaitTimer = patrolWaitTime;
        }
    }

    void ChasePlayer()
    {
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
        LevelManager level = FindFirstObjectByType<LevelManager>();


        if (!other.CompareTag("Player")) return;

        var playerAnim = other.GetComponent<AutoRunnerAnimation>();
        if (playerAnim != null && !playerAnim.IsDead)
        {
            PlayAnimation(attackHash);
            playerAnim.TakeDamage(playerAnim.maxHealth); // Ensure player dies
            if (level != null)
                level.PlayerDied();
        }
    }

    private void PlayAnimation(int hash)
    {
        if (animator == null) return;
        if (currentAnimHash == hash) return;

        if (animator.HasState(0, hash))
        {
            animator.CrossFade(hash, 0.1f);
            currentAnimHash = hash;
        }
    }
}
