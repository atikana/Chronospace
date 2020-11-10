using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float volume = 0.5f;
    private AudioSource soundEffectsAudioSource, musicAudioSource;
    private AudioClip doubleJumpClip, jumpClip, dashClip, timeWarpClip, pendulumClip, grapplingClip;

    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        soundEffectsAudioSource = audioSources[0];
        musicAudioSource = audioSources[1];

        jumpClip = Resources.Load<AudioClip>("JUMP_2");
        doubleJumpClip = Resources.Load<AudioClip>("JUMP_1");
        dashClip = Resources.Load<AudioClip>("DASH_FORWARD");
        timeWarpClip = Resources.Load<AudioClip>("TIME_SLOWDOWN");
        pendulumClip = Resources.Load<AudioClip>("PENDULUM");
        grapplingClip = Resources.Load<AudioClip>("HOOKSHOT");

        // Default volume when game starts.
        SetVolume(0.5f);
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;

        // Manually set the music audioSource volume.
        musicAudioSource.volume = 0.5f * newVolume;
    }

    public void PlayJumpSound()
    {
        if (soundEffectsAudioSource && jumpClip)
        {
            soundEffectsAudioSource.PlayOneShot(jumpClip, volume);
        }
    }

    public void PlayDoubleJumpSound()
    {
        if (soundEffectsAudioSource && doubleJumpClip)
        {
            soundEffectsAudioSource.PlayOneShot(doubleJumpClip, volume);
        }
    }

    public void PlayDashSound()
    {
        if (soundEffectsAudioSource && dashClip)
        {
            soundEffectsAudioSource.PlayOneShot(dashClip, volume);
        }
    }

    public void PlayTimeWarpSound()
    {
        if (soundEffectsAudioSource && timeWarpClip)
        {
            soundEffectsAudioSource.PlayOneShot(timeWarpClip, volume);
        }
    }

    public void PlayGrapplingSound()
    {
        if (soundEffectsAudioSource && grapplingClip)
        {
            soundEffectsAudioSource.PlayOneShot(grapplingClip, volume);
        }
    }

    public void PlayPendulumSound(Vector3 pendulumLocation)
    {
        if (pendulumClip)
        {
            /* Use PlayClipAtPoint to make pendulum noise sound
             * like it's originating at the pendulum's location.
             */
            AudioSource.PlayClipAtPoint(pendulumClip, pendulumLocation, volume);
        }
    }
}
