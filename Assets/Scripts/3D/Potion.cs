using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Potion : MonoBehaviour
{
    [Header("Coin Settings")]
    public GameObject hiddenCoinsGroup;

    [Header("Magnet Settings")]
    public float magnetDuration = 6f;
    public GameObject coinDetectorObj; // Assign in inspector or find by tag

    AudioManager_3D audioManager;

    private void Awake()
    {
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        if (audioObj != null)
            audioManager = audioObj.GetComponent<AudioManager_3D>();

        // Find coin detector if not assigned
        if (coinDetectorObj == null)
        {
            coinDetectorObj = GameObject.FindGameObjectWithTag("Coin Detector");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (audioManager != null)
            audioManager.PlaySFX(audioManager.powerUp);

        // Reveal hidden coins
        if (hiddenCoinsGroup != null)
            hiddenCoinsGroup.SetActive(true);

        // Activate magnet effect
        StartCoroutine(ActivateMagnet());

        // Destroy potion visual
        Destroy(gameObject);
    }

    IEnumerator ActivateMagnet()
    {
        if (coinDetectorObj != null)
        {
            coinDetectorObj.SetActive(true);
            Debug.Log("Magnet activated!");

            yield return new WaitForSeconds(5);

            coinDetectorObj.SetActive(false);
            Debug.Log("Magnet deactivated!");
        }
    }
}