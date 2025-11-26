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



    public static bool GlobalFreeze = false;


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

        if (animController != null) 
            animController.TriggerVictory();

        if (audioManager != null)   
            audioManager.PlaySFX(audioManager.victory);

        if (playerMover != null)    
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

        if (audioManager != null)
            audioManager.PlaySFX(audioManager.death);

        if (playerMover != null) 
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
