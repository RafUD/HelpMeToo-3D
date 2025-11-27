using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject fadeOut;

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public void StartGame()
    {
        StartCoroutine(StartButton());
    }

    public void Exit(){
        Application.Quit();
    }

    IEnumerator StartButton()
    {
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Niveau");

    }
}
