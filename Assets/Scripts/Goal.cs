using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private AudioManager_2D audioManager; // Reference to AudioManager_2D
    [SerializeField] private Enemy enemy; // Reference to enemy (to start moving later)

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager_2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Level complete!");
            StartCoroutine(LevelCompleteSequence(collision.gameObject));
        }
    }

    private IEnumerator LevelCompleteSequence(GameObject player)
    {
        // disable movement/shooting/animation
        Movement move = player.GetComponent<Movement>();
        Shooting shoot = player.GetComponent<Shooting>();
        Animator animator = player.GetComponent<Animator>();

        if (move != null) move.enabled = false;
        if (shoot != null) shoot.enabled = false;
        if (animator != null) animator.SetBool("IsStopping", true);

        yield return new WaitForSeconds(1f); // short pause before enemy starts moving



        // Stop the music
        if (audioManager != null)
        {
            audioManager.StopMusic();
        }


        //  Start enemy movement after short delay
        if (enemy != null && !enemy.enabled)
        {
            enemy.enabled = true;
            Debug.Log("Enemy started moving!");

            audioManager?.PlaySFX(audioManager.masterSFX, 2f);

        }

        yield return new WaitForSeconds(2f); // wait before continuing

        GetComponent<Collider2D>().enabled = false; // optional
        transform.position += Vector3.up * 10f;    // move it away so player can pass

        if (move != null) move.enabled = true;
        if (animator != null) animator.SetBool("IsStopping", false);


        yield return new WaitForSeconds(3f); // small pause before loading next level

        // Load next scene in build order
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Make sure next scene exists in build settings
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more scenes in build order!");
        }
    }
}
