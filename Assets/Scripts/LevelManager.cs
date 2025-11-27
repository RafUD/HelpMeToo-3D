using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public AutoRunnerAnimation animController;
    public AutoRunner playerMover;
    public AudioManager audioManager;

    public float victoryDelay = 6f;
    public float deathDelay = 3f;

    bool levelEnded = false;

    void Awake()
    {
        if (animController == null)
            animController = FindFirstObjectByType<AutoRunnerAnimation>();

        if (playerMover == null)
            playerMover = FindFirstObjectByType<AutoRunner>();

        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();
    }

    // PLAYER WINS
    public void WinLevel()
    {
        if (levelEnded) return;
        levelEnded = true;

        if (animController != null)
            animController.TriggerVictory();

        if (audioManager != null)
            audioManager.PlaySFX(audioManager.victory);

        if (playerMover != null)
            playerMover.enabled = false;

        ItemsManager.coinsCollected = 0;

        Invoke(nameof(LoadNextLevel), victoryDelay);
    }

    // PLAYER LOSES
    public void PlayerDied()
    {
        if (levelEnded) return;
        levelEnded = true;

        if (audioManager != null)
            audioManager.PlaySFX(audioManager.death);

        if (playerMover != null)
            playerMover.enabled = false;

        ItemsManager.coinsCollected = 0;
        //Find("FadeOut"))
        Invoke(nameof(RestartLevel), deathDelay);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene(0); // menu
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
