using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private AudioManager_2D audioManager;
    private Button button;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager_2D>();
        button = GetComponent<Button>();
    }

    //  Play hover SFX
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioManager != null && audioManager.hoverSFX != null)
            audioManager.PlaySFX(audioManager.hoverSFX);
    }

    //  Play click SFX
    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioManager == null) return;

        string buttonName = button.name.ToLower();

        // Cancel buttons (like "quit", "back", "exit", "resume")
        if (buttonName.Contains("quit") || buttonName.Contains("back") || buttonName.Contains("exit") || buttonName.Contains("resume"))
        {
            if (audioManager.cancelUISFX != null)
                audioManager.PlaySFX(audioManager.cancelUISFX);
        }
        else
        {
            // All other buttons = confirm sound
            if (audioManager.confirmUISFX != null)
                audioManager.PlaySFX(audioManager.confirmUISFX);
        }
    }
}
