using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public AutoRunnerAnimation animController;
    public AutoRunner playerMover;
    public AudioManager audioManager;

    public float victoryDelay = 2f;
    public float deathDelay = 2f;

    bool levelEnded = false;



    public static bool GlobalFreeze = false; // all enemies check this


    void Awake()
    {

        GlobalFreeze = false; // Reset when level loads


        if (animController == null)
            animController = FindFirstObjectByType<AutoRunnerAnimation>();

        if (playerMover == null)
            playerMover = FindFirstObjectByType<AutoRunner>(); 

        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();
    }


    //      PLAYER WINS
    public void WinLevel()
    {
        if (levelEnded) return;
        levelEnded = true;

        GlobalFreeze = true;

        if (animController != null)  // ← Add this
            animController.TriggerVictory();

        if (audioManager != null)    // ← Add this
            audioManager.PlaySFX(audioManager.victory);

        if (playerMover != null)     // ← Add this
            playerMover.enabled = false;


        ItemsManager.coinsCollected = 0;


        Invoke(nameof(LoadNextLevel), victoryDelay);
    }

    //      PLAYER LOSES
    public void PlayerDied()
    {
        if (levelEnded) return;
        levelEnded = true;

        GlobalFreeze = true;

        if (audioManager != null)  // ← Null check here
            audioManager.PlaySFX(audioManager.death);

        if (playerMover != null)   // ← Null check here
            playerMover.enabled = false;


        ItemsManager.coinsCollected = 0;


        Invoke(nameof(RestartLevel), deathDelay);
    }

    //  Scene Management
    void LoadNextLevel()
    {
        SceneManager.LoadScene(0); //menu, un seul niveau
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
