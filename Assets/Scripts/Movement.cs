using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumping = 16f;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;

    private bool isFacingRight = true;
    private bool isDead = false;
    private bool isStopped = false;
    private bool isStunned = false;
    private bool shootingPaused = false; // shift toggle state

    [Header("Death Physics")]
    [SerializeField] private float deathBounceForce = 24f;
    [SerializeField] private float deathSpinTorque = 300f;

    public ParticleSystem dustFX;

    private Shooting shooting;

    //Audio
    AudioManager_2D audioManager;
    private AudioSource sfxSource; // Reference to the AudioSource component


    public bool inputLocked = false;


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager_2D>();
        sfxSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }


    private void Start()
    {
        shooting = GetComponent<Shooting>();
    }
    private void Update()
    {

        if (inputLocked) return;


        if (isDead) return;

        HandleFlip();
        HandleJump();
        HandleMovementStop(); // down key logic
        HandleShootingToggle(); // shift key toggle

        // Play footstep SFX whenever the player is grounded, not stopped, not stunned, and not dead
        bool shouldPlayFootsteps = !isStopped && !isStunned && IsGrounded();

        if (shouldPlayFootsteps)
        {
            if (sfxSource.clip != audioManager.playerMovementSFX || !sfxSource.isPlaying)
            {
                sfxSource.clip = audioManager.playerMovementSFX;
                sfxSource.loop = true;
                sfxSource.volume = 0.01f; 
                sfxSource.Play();
                Debug.Log("Play footstep SFX");
            }
            DustFX();
        }
        else
        {
            if (sfxSource.isPlaying && sfxSource.clip == audioManager.playerMovementSFX)
            {
                sfxSource.Stop();
                sfxSource.loop = false;
                sfxSource.clip = null;
                sfxSource.volume = 1f; // Restore volume for other SFX
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isStopped || isStunned) return;

        float direction = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded() && !isStopped)
        {
            Debug.Log("Player Jumped");
            audioManager.PlaySFX(audioManager.jumpSFX);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumping);
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    // DOWN key - stops movement only
    private void HandleMovementStop()
    {
        bool stopInput = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

        if (stopInput && !isStopped)
        {
            isStopped = true;
            rb.linearVelocity = Vector2.zero;

            animator?.SetBool("IsStopping", true);

            if (dustFX != null && dustFX.isPlaying)
                dustFX.Stop();

            Debug.Log("Player stopped.");
        }
        else if (!stopInput && isStopped)
        {
            isStopped = false;

            animator?.SetBool("IsStopping", false);

            if (dustFX != null && !dustFX.isPlaying)
                dustFX.Play();

            Debug.Log(" Player resumed movement.");
        }
    }






    //  SHIFT key - toggles shooting on/off
    private void HandleShootingToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            shootingPaused = !shootingPaused;

            if (shooting != null)
                shooting.enabled = !shootingPaused;

            Debug.Log(shootingPaused ? "Shooting paused." : "Shooting resumed.");
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleFlip()
    {
        if ((!isFacingRight && Input.GetKeyDown(KeyCode.RightArrow)) ||
            (isFacingRight && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    public void PlayerDeath()
    {
        isDead = true;
        GetComponent<Collider2D>().enabled = false;

        if (shooting != null)
            shooting.enabled = false;

        rb.constraints = RigidbodyConstraints2D.None;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * deathBounceForce, ForceMode2D.Impulse);
        rb.AddTorque(deathSpinTorque, ForceMode2D.Impulse);
    }

    public void Respawn()
    {
        if (!isFacingRight) Flip();

        isDead = false;
        GetComponent<Collider2D>().enabled = true;

        if (shooting != null && !shootingPaused)
            shooting.enabled = true;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.rotation = Quaternion.identity;
    }

    private void DustFX()
    {
        if (dustFX == null) return;

        if (!dustFX.isPlaying)
            dustFX.Play();
    }
}
