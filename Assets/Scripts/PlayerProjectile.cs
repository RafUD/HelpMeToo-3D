using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float damage = 20f;
    public float lifetime = 2f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public ParticleSystem groundImpactFX;
    private bool potionPowered = false;

    private void Awake()   // use Awake instead of Start
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // assign before Start()
    }

    private void Start()
    {
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            if (groundImpactFX != null)
                Instantiate(groundImpactFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void PotionPower(bool state)
    {
        potionPowered = state;

        if (potionPowered)
        {
            transform.localScale *= 3f;
            damage *= 200f;
            lifetime *= 3f;

            if (spriteRenderer != null)
                spriteRenderer.color = new Color32(26, 0, 219, 255);
            else
                Debug.LogWarning("SpriteRenderer not found on projectile!");
        }
    }
}
