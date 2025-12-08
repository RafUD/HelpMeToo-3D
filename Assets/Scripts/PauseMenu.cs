using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool Paused = false;
    public GameObject PauseMenuUI;
    public GameObject GameOverUI;

    private AudioManager_2D audioManager;

    void Start()
    {
        Paused = false;
        Time.timeScale = 1f;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager_2D>();
    }

    void Update()
    {
        // --- Block pause input if GameOverUI is active ---
        if (GameOverUI != null && GameOverUI.activeSelf)
            return;

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
        PauseMenuUI.SetActive(true);
        Debug.Log("Pause menu canvas!!");
        Time.timeScale = 0f;
        Debug.Log("Pause: time frozen!!");
        Paused = true;

        if (audioManager != null && audioManager.pauseSFX != null)
            audioManager.PlaySFX(audioManager.pauseSFX);
    }

    public void Play()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;

        if (audioManager != null && audioManager.unpauseSFX != null)
            audioManager.PlaySFX(audioManager.unpauseSFX);
    }

    public void MainMenuButton()
    {
        Debug.Log("Return to Main Menu");
        if (audioManager != null && audioManager.cancelUISFX != null)
            audioManager.PlaySFX(audioManager.cancelUISFX);

        SceneManager.LoadScene("Main Menu");
    }
}
