using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float health = 50f;
    [SerializeField] private float speed = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    [Header("Patrol Points")]
    public GameObject pointA;
    public GameObject pointB;
    private Transform currentPoint;

    [Header("Damage to Player")]
    public int damage = 1;

    private AudioManager_2D audioManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPoint = pointB.transform; // start moving toward B

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager_2D>();
    }

    private void Update()
    {
        if (isDead) return;

        // Move toward the active point
        if (currentPoint == pointB.transform)
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);

        // Check if near target, switch direction
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            Flip();
            currentPoint = (currentPoint == pointB.transform) ? pointA.transform : pointB.transform;
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If touching player, flip direction
        if (collision.collider.CompareTag("Player"))
        {
            Flip();
            currentPoint = (currentPoint == pointB.transform) ? pointA.transform : pointB.transform;
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pointA.transform.position, 0.3f);
            Gizmos.DrawSphere(pointB.transform.position, 0.3f);
            Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
            rb.linearVelocity = Vector2.zero;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (animator != null)
            animator.SetTrigger("Hit");

        // --- Play impact sound ---
        if (audioManager != null && audioManager.enemyImpactSFX != null)
            audioManager.PlaySFX(audioManager.enemyImpactSFX, 0.4f);

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;

        if (animator != null)
            animator.SetTrigger("Die");

        // --- Play death sound ---
        if (audioManager != null && audioManager.deathEnemySFX != null)
            audioManager.PlaySFX(audioManager.deathEnemySFX, 0.6f);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        Destroy(gameObject, 1.2f); // Let death animation play
    }
}
