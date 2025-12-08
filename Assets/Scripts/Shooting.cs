using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] private Transform shootingPoint;   // where bullets spawn
    [SerializeField] private GameObject bulletPrefab;   // projectile prefab
    [SerializeField] private float fireRate = 0.25f;    // time between shots

    private float nextFireTime = 0f;
    private PlayerManager playerManager;
    private AudioManager_2D audioManager;
    private Animator animator;

    private void Start()
    {
        playerManager = FindAnyObjectByType<PlayerManager>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager_2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Auto-fire loop — but Movement.cs can toggle this component on/off using Shift
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, shootingPoint.position, shootingPoint.rotation);

        // Power-up check
        PlayerProjectile projectile = bullet.GetComponent<PlayerProjectile>();
        if (projectile != null)
        {
            projectile.PotionPower(playerManager.isPoweredUp);
        }

        // Play correct sound
        if (playerManager.isPoweredUp)
            audioManager.PlaySFX(audioManager.empoweredShootSFX, 0.5f);
        else
            audioManager.PlaySFX(audioManager.shootSFX, 0.3f);
    }
}
