using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class BossEnemy : MonoBehaviour
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

    [Header("Boss Specific Settings")]
    public int coinsRequiredToDefeat = 50;

    private Rigidbody rb;
    private Animator animator;

    private Transform currentPatrolTarget;
    private bool isChasing;
    private float patrolWaitTimer;

    private int currentAnimHash = -1;

    private readonly int runHash = Animator.StringToHash("Mutant Run");
    private readonly int attackWinHash = Animator.StringToHash("Mutant Punch"); // Boss wins
    private readonly int attackLoseHash = Animator.StringToHash("Mutant Dying"); // Player wins

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentPatrolTarget = pointB;
        isChasing = false;
        patrolWaitTimer = 0f;

        PlayAnimation(runHash); // Boss always runs
    }

    void Update()
    {
        // Don't act if player is dead or boss is in animation freeze
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
                isChasing = false;
            Patrol();
            PlayAnimation(runHash);
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

        if (Vector3.Distance(transform.position, targetPos) < 0.3f)
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
        if (!other.CompareTag("Player")) return;

        var playerAnim = other.GetComponent<AutoRunnerAnimation>();
        if (playerAnim == null || playerAnim.IsDead) return;

        int playerCoins = ItemsManager.coinsCollected;
        LevelManager level = FindFirstObjectByType<LevelManager>();

        if (playerCoins >= coinsRequiredToDefeat)
        {
            // Player defeats boss
            PlayAnimation(attackLoseHash);
            Debug.Log($"Boss defeated! Player coins: {playerCoins}/{coinsRequiredToDefeat}");

            if (level != null)
                level.WinLevel();

            Destroy(gameObject, 2f); // Destroy after animation
        }
        else
        {
            // Boss wins
            PlayAnimation(attackWinHash);
            Debug.Log($"Boss wins! Player coins: {playerCoins}/{coinsRequiredToDefeat}");

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.position, 0.5f);
        }
    }
}
