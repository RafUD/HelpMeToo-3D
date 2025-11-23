using System.Collections;
using UnityEngine;

public class IntroCamera : MonoBehaviour
{
    public float delayInSeconds = 5f; // Set the delay duration in the Inspector
    public GameObject followPlayerCam;

    void Start()
    {
        // Start the coroutine when the script begins
        StartCoroutine(DeactivateObjectAfterDelay());
    }

    IEnumerator DeactivateObjectAfterDelay()
    {
        // Wait for the specified duration
        yield return new WaitForSeconds(delayInSeconds);

        // Deactivate the GameObject this script is attached to
        gameObject.SetActive(false);

        followPlayerCam.SetActive(true);
    }

}
