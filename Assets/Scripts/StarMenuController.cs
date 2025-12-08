using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerIDInput;

    public void Play2D()
    {
        SavePlayerID();
        SceneManager.LoadScene("Main Menu 2D");
    }

    public void Play3D()
    {
        SavePlayerID();
        SceneManager.LoadScene("Main Menu 3D");
    }

    private void SavePlayerID()
    {
        if (string.IsNullOrWhiteSpace(playerIDInput.text))
        {
            Debug.LogWarning("Player ID is empty!");
            return;
        }

        PlayerPrefs.SetString("PlayerID", playerIDInput.text);
        Debug.Log("Player ID saved: " + playerIDInput.text);
    }
}