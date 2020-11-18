using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float volume = 0.5f;
    private AudioSource soundEffectsAudioSource, highPitchSoundEffectsAudioSource;
    private AudioClip doubleJumpClip, jumpClip, jumpLandingClip, dashClip, timeWarpClip, pendulumClip,
        grapplingClip, countDownClip, rewindClip, checkpointClip, bulletClip1, bulletClip2, bulletClip3;

    // High pass filter to be applied during time warp.
    public AudioHighPassFilter highPassFilter;

    // Low pass filter to be applied during pause.
    public AudioLowPassFilter lowPassFilter;

    void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        soundEffectsAudioSource = audioSources[0];
        highPitchSoundEffectsAudioSource = audioSources[1];
        Debug.Log("Pitches:  " + soundEffectsAudioSource.pitch + " " + highPitchSoundEffectsAudioSource.pitch);

        jumpClip = Resources.Load<AudioClip>("JUMP");
        jumpLandingClip = Resources.Load<AudioClip>("LAND");
        dashClip = Resources.Load<AudioClip>("DASH_SHORT1");
        timeWarpClip = Resources.Load<AudioClip>("TIME_SLOWDOWN");
        pendulumClip = Resources.Load<AudioClip>("PENDULUM");
        grapplingClip = Resources.Load<AudioClip>("HOOKSHOT");
        countDownClip = Resources.Load<AudioClip>("COUNTDOWN");
        rewindClip = Resources.Load<AudioClip>("DEATH_REWIND");
        checkpointClip = Resources.Load<AudioClip>("CHECKPOINT");
        bulletClip1 = Resources.Load<AudioClip>("TURRET_BULLET_1");
        bulletClip2 = Resources.Load<AudioClip>("TURRET_BULLET_2");
        bulletClip3 = Resources.Load<AudioClip>("TURRET_BULLET_3");

        // Default volume when game starts.
        SetVolume(volume);
    }

    public void SetHighPassFilterEnabled(bool enabled)
    {
        highPassFilter.enabled = enabled;
    }

    public void SetLowPassFilterEnabled(bool enabled)
    {
        lowPassFilter.enabled = enabled;
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;

        // Manually set the music audioSource volume.
        soundEffectsAudioSource.volume = 0.5f * newVolume;
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
        // Play the jump sound with a higher pitch.
        if (highPitchSoundEffectsAudioSource && doubleJumpClip)
        {
            highPitchSoundEffectsAudioSource.PlayOneShot(doubleJumpClip, volume);
        }
    }

    public void PlayJumpLandingSound()
    {
        if (soundEffectsAudioSource && jumpLandingClip)
        {
            soundEffectsAudioSource.PlayOneShot(jumpLandingClip, volume);
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

    public void PlayCountdownSound()
    {
        if (soundEffectsAudioSource && countDownClip)
        {
            soundEffectsAudioSource.PlayOneShot(countDownClip, volume);
        }
    }

    public void PlayRewindSound()
    {
        if (soundEffectsAudioSource && rewindClip)
        {
            soundEffectsAudioSource.PlayOneShot(rewindClip, Mathf.Min(volume * 2f, 1f));
        }
    }

    public void PlayCheckpointSound()
    {
        if (soundEffectsAudioSource && checkpointClip)
        {
            soundEffectsAudioSource.PlayOneShot(checkpointClip, volume);
        }
    }

    public void PlayBulletSound()
    {
        if (soundEffectsAudioSource && bulletClip1 && bulletClip2 && bulletClip3)
        {
            int clipNumber = Random.Range(0, 3);
            switch (clipNumber)
            {
                case 0:
                    soundEffectsAudioSource.PlayOneShot(bulletClip1, volume);
                    break;
                case 1:
                    soundEffectsAudioSource.PlayOneShot(bulletClip2, volume);
                    break;
                case 2:
                    soundEffectsAudioSource.PlayOneShot(bulletClip3, volume);
                    break;
            }
        }
    }
}
