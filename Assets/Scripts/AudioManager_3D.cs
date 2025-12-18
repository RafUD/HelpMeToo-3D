//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class AudioManager_3D : MonoBehaviour
//{
//    [Header("Audio Sources")]
//    [SerializeField] private AudioSource music;   // Music of the level
//    [SerializeField] private AudioSource sfx;     

//    [Header("Audio Clips")]
//    public AudioClip background;   
//    public AudioClip death;        
//    public AudioClip jump;         
//    public AudioClip coin;         
//    public AudioClip fall;         // Fall sound
//    public AudioClip playerHit;    
//    public AudioClip victory;      // Player win sound
//    public AudioClip defeat;       // Lose sound
//    public AudioClip powerUp;      
//    public AudioClip powerDown;    
//    public AudioClip buttonClick;  
//    public AudioClip mirrorBreak;  
//    public AudioClip playerMove;   //player running sound

//    public AudioClip evilLaugh;
//    public AudioClip BossRoar;
//    public AudioClip enemyDie;

//    private void Start()
//    {
//        // Play background music at start
//        music.clip = background;
//        music.Play();
//    }

//    public void PlaySFX(AudioClip clip)
//    {
//        sfx.PlayOneShot(clip);
//    }
//}


using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager_3D : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;


    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip coin;
    public AudioClip powerUp;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip fall;         // Fall sound
    public AudioClip playerHit;
    public AudioClip victory;      // Player win sound
    public AudioClip defeat;       // Lose sound
    public AudioClip powerDown;
    public AudioClip buttonClick;
    public AudioClip mirrorBreak;
    public AudioClip playerMove;   //player running sound

    public AudioClip evilLaugh;
    public AudioClip BossRoar;
    public AudioClip enemyDie;
    public static AudioManager_3D Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Subscribe to scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Play background music if assigned
        if (musicSource != null && background != null)
        {
            musicSource.clip = background;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop all audio when a new scene loads
        StopAllAudio();

        // Optionally restart music for the new scene
        if (musicSource != null && background != null)
        {
            musicSource.Play();
        }

        Debug.Log($"[AudioManager] Scene changed to: {scene.name}");
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StopAllAudio()
    {
        if (musicSource != null)
            musicSource.Stop();

        if (sfxSource != null)
            sfxSource.Stop();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void StopSFX()
    {
        if (sfxSource != null)
            sfxSource.Stop();
    }
}