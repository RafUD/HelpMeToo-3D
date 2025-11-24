using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Sources")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource sfx;

    [Header("Audio Clips")]
    public AudioClip background;
    public AudioClip death;
    public AudioClip jump;
    public AudioClip coin;
    public AudioClip fall;
    public AudioClip playerHit;
    public AudioClip victory;
    public AudioClip defeat;
    public AudioClip powerUp;
    public AudioClip powerDown;
    public AudioClip buttonClick;
    public AudioClip mirrorBreak;



    private void Start()
    {
        music.clip = background;
        music.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfx.PlayOneShot(clip);
    }
}
