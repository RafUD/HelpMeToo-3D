using System.Collections;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public GameObject coinDetectorObj;
    public float magnetDuration = 6f;

    void Start()
    {
        coinDetectorObj = GameObject.FindGameObjectWithTag("Coin Detector");
        if (coinDetectorObj != null)
            coinDetectorObj.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ActivateMagnet());

            // Destroy visual (keep parent for coroutine)
            Transform visual = transform.GetChild(0);
            if (visual != null)
                Destroy(visual.gameObject);
        }
    }

    IEnumerator ActivateMagnet()
    {
        if (coinDetectorObj != null)
        {
            coinDetectorObj.SetActive(true);
            yield return new WaitForSeconds(magnetDuration);
            coinDetectorObj.SetActive(false);
        }

        Destroy(gameObject); // Clean up magnet object
    }
}