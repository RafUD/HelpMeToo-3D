using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Vector3 spawnPosition;

    [Header("Lives")]
    public int maxLives = 3;
    private int currentLives;

    [Header("References")]
    public HeartsUI heartsUI;
    public PlayerProjectile projectile;
    public GameObject GameOverCanvas;

    [Header("Damage Feedback")]
    public float knockbackX = 5f;
    public float miniJumpY = 4f;
    public float invincibilityTime = 1f;
    public float blinkSpeed = 0.1f;

    private bool isInvincible = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Collectibles")]
    private int coinCounter = 0;
    public TMP_Text counterText;

    [Header("Power-Up")]
    public bool isPoweredUp = false;
    public float powerUpDuration = 5f;

    private AudioManager_2D audioManager; // Reference to AudioManager_2D

    private void Start()
    {
        spawnPosition = transform.position;
        currentLives = maxLives;
        heartsUI.SetMaxHearts(maxLives);

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager_2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // --- ENEMY HIT ---
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy && !isInvincible)
        {
            TakeDamage(enemy.damage);

            // Knockback + mini jump
            Vector2 knockDir = (transform.position - enemy.transform.position).normalized;
            float direction = Mathf.Sign(knockDir.x);
            rb.linearVelocity = new Vector2(direction * knockbackX, miniJumpY);

            GetComponent<Movement>()?.Stun(0.4f);

            audioManager?.PlaySFX(audioManager.playerImpackSFX, 1.2f);

            StartCoroutine(InvincibilityFlash());
        }

        // --- COIN ---
        else if (collision.collider.CompareTag("Coin") && collision.gameObject.activeSelf)
        {
            collision.gameObject.SetActive(false);
            coinCounter++;
            counterText.text = coinCounter.ToString();
            audioManager?.PlaySFX(audioManager.coinSFX);
        }

        // --- FALL COLLIDER ---
        else if (collision.collider.CompareTag("FallCollider"))
        {
            TakeDamage(1);
            Die();
            audioManager?.PlaySFX(audioManager.fallSFX);
        }

        // --- HEART (HEAL) ---
        else if (collision.collider.CompareTag("Heart") && collision.gameObject.activeSelf)
        {
            collision.gameObject.SetActive(false);
            currentLives = maxLives;
            heartsUI.UpdateHearts(currentLives);
            audioManager?.PlaySFX(audioManager.healSFX);
        }

        // --- POTION (POWER-UP) ---
        else if (collision.collider.CompareTag("Potion") && collision.gameObject.activeSelf)
        {
            collision.gameObject.SetActive(false);
            StartCoroutine(InvincibilityFlash());
            StartCoroutine(PotionPowerUp());
            audioManager?.PlaySFX(audioManager.powerUpSFX);
        }
    }

    private void TakeDamage(int damage)
    {
        currentLives -= damage;
        heartsUI.UpdateHearts(currentLives);
        GetComponent<Animator>()?.SetTrigger("Hit");

        if (currentLives <= 0)
        {
            // Play Game Over Music (matches latest AudioManager_2D)
            audioManager?.PlayMusic(audioManager.gameOverMusic);

            Die();
            RestartUI();
        }
    }

    private void Die()
    {
        GetComponent<Movement>()?.PlayerDeath();
        StartCoroutine(RespawnPlayer());
    }

    private IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(2.2f);
        transform.position = spawnPosition;
        GetComponent<Movement>()?.Respawn();
    }

    private void RestartUI()
    {
        GameOverCanvas.SetActive(true);
        Time.timeScale = 0f;

        // Use gameOverMusic instead of deathScreenMusic
        audioManager?.PlayMusic(audioManager.gameOverMusic);
    }

    private IEnumerator PotionPowerUp()
    {
        isPoweredUp = true;
        yield return new WaitForSeconds(powerUpDuration);
        isPoweredUp = false;
    }

    private IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        float elapsed = 0f;

        while (elapsed < invincibilityTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkSpeed);
            elapsed += blinkSpeed;
        }

        sr.enabled = true;
        isInvincible = false;
    }
}
