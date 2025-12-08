using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ObstacleCollision : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 1;
    public int coinPenalty = 1;
    public bool destroyOnHit = false;
    public float knockbackDistance = 10f;
    public float pushOutBuffer = 2f; // extra distance to place player outside collider
    public bool addKinematicRigidbody = true;

    AudioManager_3D audioManager;
    LevelManager levelManager;
    Collider ownCollider;
    Rigidbody rb;
    int lastHitFrame = -1;
    GameObject lastHitTarget;
    Coroutine stunRoutine;

    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager_3D>();
        levelManager = FindFirstObjectByType<LevelManager>();
        ownCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        EnsureRigidbody();
    }

    void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.collider);
    }

    void HandleHit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (Time.frameCount == lastHitFrame && other.gameObject == lastHitTarget)
            return;

        lastHitFrame = Time.frameCount;
        lastHitTarget = other.gameObject;

        bool playerDied = ApplyDamage(other.gameObject);
        PushPlayerOut(other);
        ApplyKnockback(other);
        ApplyCoinPenalty();

        PlayImpactSfx(playerDied);

        if (!playerDied)
            StartStun(other.gameObject);

        if (playerDied && levelManager != null && other.GetComponent<AutoRunnerAnimation>() == null)
        {
            // AutoRunnerAnimation already notifies LevelManager; fall back for other controllers.
            levelManager.PlayerDied();
        }

        if (destroyOnHit)
            Destroy(gameObject);
    }

    bool ApplyDamage(GameObject player)
    {
        bool died = false;

        var animController = player.GetComponent<animationStateController>();
        if (animController != null)
        {
            animController.TakeDamage(damage);
            died |= animController.IsDead;
        }

        var autoRunnerAnim = player.GetComponent<AutoRunnerAnimation>();
        if (autoRunnerAnim != null)
        {
            autoRunnerAnim.TakeDamage(damage);
            died |= autoRunnerAnim.IsDead;
        }

        return died;
    }

    void ApplyKnockback(Collider other)
    {
        if (knockbackDistance <= 0f) return;

        var controller = other.GetComponent<CharacterController>();
        if (controller == null) return;

        Vector3 direction = -other.transform.forward;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f && ownCollider != null)
        {
            direction = (other.transform.position - ownCollider.bounds.center);
            direction.y = 0f;
        }

        if (direction.sqrMagnitude < 0.001f)
            direction = Vector3.back;

        controller.Move(direction.normalized * knockbackDistance);
    }

    void PushPlayerOut(Collider other)
    {
        if (ownCollider == null) return;

        var controller = other.GetComponent<CharacterController>();
        if (controller == null) return;

        Bounds obstacleBounds = ownCollider.bounds;
        Vector3 playerPos = other.bounds.center;

        Vector3 direction = (playerPos - obstacleBounds.center);
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f)
            direction = Vector3.back; // fallback

        float pushDistance = obstacleBounds.extents.magnitude + controller.radius + pushOutBuffer;
        Vector3 targetPos = obstacleBounds.center + direction.normalized * pushDistance;

        Vector3 move = targetPos - playerPos;
        move.y = 0f; // only horizontal push
        controller.Move(move);
    }

    void StartStun(GameObject player)
    {
        if (stunRoutine != null)
            StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(StunPlayer(player));
    }

    IEnumerator StunPlayer(GameObject player)
    {
        var autoRunner = player.GetComponent<AutoRunner>();
        var runnerMover = player.GetComponent<RunnerMover>();
        var autoAnim = player.GetComponent<AutoRunnerAnimation>();
        var legacyAnim = player.GetComponent<animationStateController>();
        var animator = player.GetComponent<Animator>();

        // Force idle pose
        if (animator != null)
            animator.CrossFade("Idle", 0.05f);

        // Pause movement/animation scripts
        if (autoAnim != null) autoAnim.enabled = false;
        if (legacyAnim != null) legacyAnim.enabled = false;
        if (autoRunner != null) autoRunner.enabled = false;
        if (runnerMover != null) runnerMover.enabled = false;

        yield return new WaitForSeconds(0.5f);

        // Resume
        if (autoAnim != null) autoAnim.enabled = true;
        if (legacyAnim != null) legacyAnim.enabled = true;
        if (autoRunner != null) autoRunner.enabled = true;
        if (runnerMover != null) runnerMover.enabled = true;

        // Force back to movement state matching current stage
        if (animator != null)
            CrossFadeToCurrentStage(animator);

        stunRoutine = null;
    }

    void CrossFadeToCurrentStage(Animator animator)
    {
        string stateName;
        switch (ItemsManager.CurrentStage)
        {
            case ItemsManager.SpeedStage.Running:
                stateName = "Running";
                break;
            case ItemsManager.SpeedStage.Jogging:
                stateName = "Jog Forward";
                break;
            default:
                stateName = "Walking";
                break;
        }

        animator.CrossFade(stateName, 0.05f);
    }

    void EnsureRigidbody()
    {
        if (!addKinematicRigidbody) return;
        if (rb != null) return;

        rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void ApplyCoinPenalty()
    {
        if (coinPenalty <= 0) return;

        ItemsManager.coinsCollected = Mathf.Max(0, ItemsManager.coinsCollected - coinPenalty);
    }

    void PlayImpactSfx(bool killed)
    {
        if (audioManager == null) return;

        AudioClip clip = null;
        if (killed && audioManager.death != null)
        {
            clip = audioManager.death;
        }
        else if (audioManager.playerHit != null)
        {
            clip = audioManager.playerHit;
        }
        else if (audioManager.powerDown != null)
        {
            clip = audioManager.powerDown;
        }

        if (clip != null)
            audioManager.PlaySFX(clip);
    }
}
