using UnityEngine;

public class Coin : MonoBehaviour
{
    [HideInInspector] public Transform playerTransform;
    public float moveSpeed = 17f;

    private CoinMove coinMoveScript;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        coinMoveScript = GetComponent<CoinMove>();

        // Disable movement initially (magnet inactive)
        if (coinMoveScript != null)
            coinMoveScript.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // When coin enters magnet radius
        if (other.CompareTag("Coin Detector"))
        {
            if (coinMoveScript != null)
                coinMoveScript.enabled = true; // Start moving toward player
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When magnet deactivates
        if (other.CompareTag("Coin Detector"))
        {
            if (coinMoveScript != null)
                coinMoveScript.enabled = false; // Stop moving
        }
    }
}