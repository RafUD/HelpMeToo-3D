using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    AudioManager audioManager;  

    private void Awake()
    {
        // Locate the object tagged "Audio" and get AudioManager
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null)
            audioManager = audioObj.GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only the player can collect coins
        if (other.CompareTag("Player"))
        {
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.coin);

            // Save old speed stage
            ItemsManager.SpeedStage previousStage = ItemsManager.CurrentStage;

            // Increase coin count
            ItemsManager.coinsCollected += 1;

            // Check if a new speed stage was reached
            ItemsManager.SpeedStage newStage = ItemsManager.CurrentStage;
            if (newStage != previousStage && audioManager != null)
            {
                audioManager.PlaySFX(audioManager.powerUp); // Play speed-upgrade sound
            }

            // Remove the coin from the scene
            Destroy(gameObject);
        }
    }
}
