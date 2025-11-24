using UnityEngine;

public class CollectCoin : MonoBehaviour
{

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioManager.PlaySFX(audioManager.coin);
            ItemsManager.coinsCollected += 1;
            Destroy(gameObject);
        }

    }
}
