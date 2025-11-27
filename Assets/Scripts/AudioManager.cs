using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource music;   // Music of the level
    [SerializeField] private AudioSource sfx;     

    [Header("Audio Clips")]
    public AudioClip background;   
    public AudioClip death;        
    public AudioClip jump;         
    public AudioClip coin;         
    public AudioClip fall;         // Fall sound
    public AudioClip playerHit;    
    public AudioClip victory;      // Player win sound
    public AudioClip defeat;       // Lose sound
    public AudioClip powerUp;      
    public AudioClip powerDown;    
    public AudioClip buttonClick;  
    public AudioClip mirrorBreak;  
    public AudioClip playerMove;   //player running sound

    private void Start()
    {
        // Play background music at start
        music.clip = background;
        music.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }
}
