using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource soundEffectsAudioSource, backgroundMusicAudioSource;
    public AudioClip doubleJumpClip, jumpClip, dashClip, timeWarpClip, pendulumClip;

    void Start()
    {
        soundEffectsAudioSource = GetComponent<AudioSource>();

        jumpClip = Resources.Load<AudioClip>("JUMP_2");
        doubleJumpClip = Resources.Load<AudioClip>("JUMP_1");
        dashClip = Resources.Load<AudioClip>("DASH_FORWARD");
        timeWarpClip = Resources.Load<AudioClip>("TIME_SLOWDOWN");
        pendulumClip = Resources.Load<AudioClip>("PENDULUM");

        // Play background music
        PlayLevel1Music();
    }

    public void PlayJumpSound()
    {
        soundEffectsAudioSource.PlayOneShot(jumpClip);
    }

    public void PlayDoubleJumpSound()
    {
        soundEffectsAudioSource.PlayOneShot(doubleJumpClip);
    }

    public void PlayDashSound()
    {
        soundEffectsAudioSource.PlayOneShot(dashClip);
    }

    public void PlayTimeWarpSound()
    {
        soundEffectsAudioSource.PlayOneShot(timeWarpClip);
    }

    public void PlayPendulumSound(Vector3 pendulumLocation)
    {
        AudioSource.PlayClipAtPoint(pendulumClip, pendulumLocation);
    }

    public void PlayLevel1Music()
    {
        backgroundMusicAudioSource.Play();
    }
}
