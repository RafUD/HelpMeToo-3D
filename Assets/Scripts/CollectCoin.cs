using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    AudioManager_3D audioManager;

    private void Awake()
    {
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null)
            audioManager = audioObj.GetComponent<AudioManager_3D>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only the player can collect coins
        if (other.CompareTag("Player"))
        {
            CollectThisCoin();
        }
    }

    // Public method so CoinMove can also call it
    public void CollectThisCoin()
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
            audioManager.PlaySFX(audioManager.powerUp);
        }

        // Remove the coin from the scene
        Destroy(gameObject);
    }
}