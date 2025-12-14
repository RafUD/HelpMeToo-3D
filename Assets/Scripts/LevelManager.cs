using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public AutoRunnerAnimation animController; 
    public AutoRunner playerMover;             
    public AudioManager_3D audioManager;          

    public float victoryDelay = 10f; // Delay before loading next level after victory
    public float deathDelay = 8f;    // Delay before restarting level after death

    bool levelEnded = false; // prevent multiple triggers

    public GameObject loseScreen;    
    public GameObject victoryScreen;

    public GameObject pauseMenu;
    public static bool Paused = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Paused)
                Play();
            else
                Stop();
        }
    }

    public void Stop()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }

    public void Play()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }

    void Awake()
    {
        // Auto-find references if not assigned
        if (animController == null)
            animController = FindFirstObjectByType<AutoRunnerAnimation>();

        if (playerMover == null)
            playerMover = FindFirstObjectByType<AutoRunner>();

        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager_3D>();
    }

    // PLAYER WINS
    public void WinLevel()
    {
        if (levelEnded) return; // Prevent multiple calls
        levelEnded = true;

        animController?.TriggerVictory();       
        audioManager?.PlaySFX(audioManager.victory); 
        if (playerMover != null) playerMover.enabled = false;

        if (victoryScreen != null) victoryScreen.SetActive(true); 

        StartCoroutine(LoadNextAfterDelay());
    }

    IEnumerator LoadNextAfterDelay()
    {
        yield return new WaitForSecondsRealtime(victoryDelay);
        ItemsManager.coinsCollected = 0; // Reset coins
        LoadMenu();
    }

    // PLAYER LOSES
    public void PlayerDied()
    {
        if (levelEnded) return; // Prevent multiple calls
        levelEnded = true;

        audioManager?.PlaySFX(audioManager.death); 
        if (playerMover != null) playerMover.enabled = false; 

        ItemsManager.coinsCollected = 0; // Reset coins

        if (loseScreen != null) loseScreen.SetActive(true); 

        //Time.timeScale = 0f; // Pause game

        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSecondsRealtime(deathDelay);

        Time.timeScale = 1f; // Resume time
        RestartLevel();
    }


    public void LoadMenu()
    {
        SceneManager.LoadScene("Main Menu 3D"); // Load menu
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}
