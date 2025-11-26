using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null)
            audioManager = audioObj.GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play coin sound
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.coin);

            // Store previous stage
            ItemsManager.SpeedStage previousStage = ItemsManager.CurrentStage;

            // Collect coin
            ItemsManager.coinsCollected += 1;

            // Check if stage upgraded (optional: play special sound)
            ItemsManager.SpeedStage newStage = ItemsManager.CurrentStage;
            if (newStage != previousStage && audioManager != null)
            {
                // Play power-up sound when reaching new speed stage
                audioManager.PlaySFX(audioManager.powerUp);
            }

            Destroy(gameObject);
        }
    }
}