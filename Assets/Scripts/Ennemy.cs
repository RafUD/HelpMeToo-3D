using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public Transform target;

    [Header("Chase Settings")]
    public float detectionRange = 10f;
    public float chaseSpeed = 5f;

    [Header("Patrol Settings")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 3f;
    public float patrolWaitTime = 1f; // pause à chaque point


    private Rigidbody rb;
    private Renderer enemyRenderer;
    private Transform currentPatrolTarget;
    private bool isChasing;
    private float patrolWaitTimer;
    private bool isWaiting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponent<Renderer>();

        // Start patrolling toward point A
        currentPatrolTarget = pointA;
    }


    void FixedUpdate()
    {
        // Check if game is frozen (player died or won)
        if (LevelManager.GlobalFreeze)
        {
            rb.linearVelocity = Vector3.zero; 
            return;
        }

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(target.position, transform.position);

        // Check behavior based on distance
        if (distanceToPlayer <= detectionRange)
        {
            if (!isChasing)
            {
                EnterChaseMode();
            }
            ChasePlayer();
        }
        else
        {
            if (isChasing)
            {
                ExitChaseMode();
            }
            Patrol();
        }
    }

    void EnterChaseMode()
    {
        isChasing = true;
        isWaiting = false;

    }

    void ExitChaseMode()
    {
        isChasing = false;


    }

    void Patrol()
    {
        // If waiting at a patrol point
        if (isWaiting)
        {
            patrolWaitTimer -= Time.fixedDeltaTime;
            if (patrolWaitTimer <= 0)
            {
                isWaiting = false;
                // Switch to the other patrol point
                currentPatrolTarget = (currentPatrolTarget == pointA) ? pointB : pointA;
            }
            return;
        }

        // Move toward current patrol target (keep same Y position)
        Vector3 targetPos = new Vector3(
            currentPatrolTarget.position.x,
            transform.position.y,
            currentPatrolTarget.position.z
        );

        // Look at target
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 5f);
        }

        // Move toward target
        Vector3 newPos = Vector3.MoveTowards(
            transform.position,
            targetPos,
            patrolSpeed * Time.fixedDeltaTime
        );
        rb.MovePosition(newPos);

        // Check if reached patrol point
        if (Vector3.Distance(transform.position, targetPos) < 0.3f)
        {
            isWaiting = true;
            patrolWaitTimer = patrolWaitTime;
        }
    }

    void ChasePlayer()
    {
        //Keep same Y position
        Vector3 targetPos = new Vector3(
            target.position.x,
            transform.position.y,
            target.position.z
        );

        // Look at player
        Vector3 direction = (targetPos - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 8f);
        }

        // Chase target
        Vector3 newPos = Vector3.MoveTowards(
            transform.position,
            targetPos,
            chaseSpeed * Time.fixedDeltaTime
        );
        rb.MovePosition(newPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !LevelManager.GlobalFreeze)
        {
            // Kill player
            var anim = other.GetComponent<animationStateController>();
            if (anim != null)
                anim.TakeDamage(999);

            // Tout s'arrête
            LevelManager.GlobalFreeze = true;

            // Notifier level manager
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            if (levelManager != null)
                levelManager.PlayerDied();
        }
    }

    // Visualisser les zones
    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Patrol
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.5f);
            Gizmos.DrawWireSphere(pointB.position, 0.5f);
        }
    }
}