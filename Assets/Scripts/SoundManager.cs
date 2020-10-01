using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource soundEffectsAudioSource, backgroundMusicAudioSource;
    public AudioClip jumpClip, dashClip, timeWarpClip;

    void Start()
    {
        //soundEffectsAudioSource = GetComponent<AudioSource>();

        jumpClip = Resources.Load<AudioClip>("Jump");
        dashClip = Resources.Load<AudioClip>("Dash");
        timeWarpClip = Resources.Load<AudioClip>("TimeWarp");

        // Play background music
        PlayLevel1Music();
    }

    public void PlayJumpSound()
    {
        soundEffectsAudioSource.PlayOneShot(jumpClip);
    }

    public void PlayDashSound()
    {
        soundEffectsAudioSource.PlayOneShot(dashClip);
    }

    public void PlayTimeWarpSound()
    {
        soundEffectsAudioSource.PlayOneShot(timeWarpClip);
    }

    public void PlayLevel1Music()
    {
        backgroundMusicAudioSource.Play();
    }
}
