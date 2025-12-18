using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public AutoRunnerAnimation animController;
    public AutoRunner playerMover;
    public AudioManager_3D audioManager;

    [Header("Delays")]
    public float victoryDelay = 10f;
    public float deathDelay = 8f;

    [Header("UI")]
    public GameObject loseScreen;
    public GameObject victoryScreen;
    public GameObject pauseMenu;

    [Header("Scenes")]
    [SerializeField] private string menuSceneName = "Main Menu 3D";

    public static bool Paused = false;
    private bool levelEnded = false;

    void Awake()
    {
        if (animController == null)
            animController = FindFirstObjectByType<AutoRunnerAnimation>();

        if (playerMover == null)
            playerMover = FindFirstObjectByType<AutoRunner>();

        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager_3D>();
    }

    void Update()
    {
        if (levelEnded) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused) Play();
            else Stop();
        }
    }

    public void Stop()
    {
        pauseMenu?.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    public void Play()
    {
        pauseMenu?.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    // =======================
    // PLAYER WINS
    // =======================
    public void WinLevel()
    {
        if (levelEnded) return;
        levelEnded = true;

        Paused = false;
        Time.timeScale = 1f;
        pauseMenu?.SetActive(false);

        animController?.TriggerVictory();
        audioManager?.PlaySFX(audioManager.victory);

        if (playerMover != null)
            playerMover.enabled = false;

        victoryScreen?.SetActive(true);

        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.SaveScore(ItemsManager.coinsCollected);
            Debug.Log($"Saved score: {ItemsManager.coinsCollected}");
        }


        StartCoroutine(LoadNextAfterDelay());
    }

    IEnumerator LoadNextAfterDelay()
    {
        yield return new WaitForSecondsRealtime(victoryDelay);
        ItemsManager.coinsCollected = 0;
        LoadMenu();
    }

    // =======================
    // PLAYER LOSES
    // =======================
    public void PlayerDied()
    {
        if (levelEnded) return;
        levelEnded = true;

        Paused = false;
        Time.timeScale = 1f;
        pauseMenu?.SetActive(false);

        audioManager?.PlaySFX(audioManager.death);

        if (playerMover != null)
            playerMover.enabled = false;

        ItemsManager.coinsCollected = 0;

        foreach (var text in FindObjectsOfType<FloatingText3D>())
            text.enabled = false;

        loseScreen?.SetActive(true);

        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSecondsRealtime(deathDelay);
        RestartLevel();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
