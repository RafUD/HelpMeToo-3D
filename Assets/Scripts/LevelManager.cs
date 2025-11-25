using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public animationStateController animController;
    public RunnerMover playerMover;
    public AudioManager audioManager;

    public float victoryDelay = 2f;
    public float deathDelay = 2f;

    bool levelEnded = false;



    public static bool GlobalFreeze = false; // all enemies check this


    void Awake()
    {

        GlobalFreeze = false; // Reset when level loads


        if (animController == null)
            animController = FindFirstObjectByType<animationStateController>();

        if (playerMover == null)
            playerMover = FindFirstObjectByType<RunnerMover>();

        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();
    }

  
    //      PLAYER WINS
    public void WinLevel()
    {

        GlobalFreeze = true;


        if (levelEnded) return;
        levelEnded = true;

        animController.TriggerVictory();
        audioManager.PlaySFX(audioManager.victory);

        // Disable movement
        playerMover.enabled = false;

        Invoke(nameof(LoadNextLevel), victoryDelay);
    }

    //      PLAYER LOSES
    public void PlayerDied()
    {
        GlobalFreeze = true;


        if (levelEnded) return;
        levelEnded = true;

        audioManager.PlaySFX(audioManager.death);
        playerMover.enabled = false;

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
