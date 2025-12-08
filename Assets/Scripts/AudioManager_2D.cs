using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager_2D : MonoBehaviour
{
    private static AudioManager_2D instance;

    [Header("----------Audio Sources----------")]
    [SerializeField]  AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("----------UI Sliders (optional)----------")]
     private Slider musicSlider;
     private Slider sfxSlider;

    [Header("----------Audio Clips (SFX)----------")]
    public AudioClip jumpSFX;
    public AudioClip shootSFX;
    public AudioClip empoweredShootSFX;
    public AudioClip enemyImpactSFX;
    public AudioClip playerImpackSFX;
    public AudioClip deathEnemySFX;
    public AudioClip powerUpSFX;
    public AudioClip healSFX;
    public AudioClip playerMovementSFX;
    public AudioClip confirmUISFX;
    public AudioClip cancelUISFX;
    public AudioClip pauseSFX;
    public AudioClip unpauseSFX;
    public AudioClip hoverSFX;
    public AudioClip coinSFX;
    public AudioClip fallSFX;

    public AudioClip masterSFX;


    [Header("----------Music Clips----------")]
    public AudioClip niveauRaf;
    public AudioClip menuMusic;
    public AudioClip niveauAhmed;
    public AudioClip gameWonMusic;
    public AudioClip gameOverMusic;

    [Header("Volume (0..1)")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;




    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
 
    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("MusicVolume"));
            // If player somehow saved 0 accidentally, use default 1
            if (musicVolume <= 0.01f)
                musicVolume = 1f;
        }
        else
        {
            musicVolume = 1f;
        }


        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat("SFXVolume"));
        else
            sfxVolume = 1f;

        // Apply to sources
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;

        // Hook sliders if assigned (safe: this adds listeners at runtime)
        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.value = musicVolume;
            // remove any previous listeners to avoid duplicates
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Play correct music for the current scene
        UpdateMusicForScene(SceneManager.GetActiveScene().name);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(musicSlider.value); });

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(sfxSlider.value); });

    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        UpdateMusicForScene(newScene.name);
    }

    private void UpdateMusicForScene(string sceneName)
    {
        if (musicSource == null) return;

        if (sceneName.Contains("Menu"))
            PlayMusic(menuMusic);
        else if (sceneName.Contains("Raf"))
            PlayMusic(niveauRaf);
        else if (sceneName.Contains("Ahmed"))
            PlayMusic(niveauAhmed);
        else if (sceneName.Contains("Won"))
            PlayMusic(gameWonMusic);
    }

    // Play one-shot sound effects (respects sfxVolume)
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume) * sfxVolume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlayGameOverMusic()
    {
        if (musicSource == null || gameOverMusic == null) return;

        musicSource.Stop();
        musicSource.clip = gameOverMusic;
        musicSource.loop = false;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void PlayGameWonMusic()
    {
        if (musicSource == null || gameWonMusic == null) return;

        musicSource.Stop();
        musicSource.clip = gameWonMusic;
        musicSource.loop = false;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    // --- Methods for UI sliders or other callers ---
    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        if (musicSource != null)
            musicSource.volume = musicVolume;

        if (musicSlider != null && musicSlider.value != value)
            musicSlider.value = musicVolume;

        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }


    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }


}


