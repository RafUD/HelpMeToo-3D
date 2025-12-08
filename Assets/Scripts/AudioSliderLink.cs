using UnityEngine;
using UnityEngine.UI;

public class AudioSliderLink : MonoBehaviour
{
    public enum AudioType { Music, SFX }
    public AudioType type;

    private Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        var manager = FindObjectOfType<AudioManager_2D>();
        if (manager == null || slider == null) return;

        // Set slider to current volume
        if (type == AudioType.Music)
        {
            slider.value = manager.musicVolume;
            slider.onValueChanged.AddListener(manager.SetMusicVolume);
        }
        else
        {
            slider.value = manager.sfxVolume;
            slider.onValueChanged.AddListener(manager.SetSFXVolume);
        }
    }
}
