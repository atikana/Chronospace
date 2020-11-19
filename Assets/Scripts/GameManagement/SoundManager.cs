using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float volume = 0.5f;
    private AudioSource soundEffectsAudioSource, highPitchSoundEffectsAudioSource, laserAudioSource;
    private AudioClip jumpClip, jumpLandingClip, dashClip, timeWarpClip, pendulumClip,
        grapplingClip, countDownClip, rewindClip, checkpointClip;
    private AudioClip[] bulletClips;

    // Keep track of which lasers are playing a sound, so that the sound turns off at the right point in time.
    private List<Vector3> lasersPlayingSound;
    
    // High pass filter to be applied during time warp.
    public AudioHighPassFilter highPassFilter;

    // Low pass filter to be applied during pause.
    public AudioLowPassFilter lowPassFilter;

    private Transform playerTransform;

    // If player is within this distance of a laser, it will play a sound.
    private const float laserSoundThreshold = 50f;

    void Awake()
    {
        lasersPlayingSound = new List<Vector3>();
    }

    void Start()
    {
        playerTransform = FindObjectOfType<PlayerControl>().transform;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        soundEffectsAudioSource = audioSources[0];
        highPitchSoundEffectsAudioSource = audioSources[1];
        laserAudioSource = audioSources[2];

        jumpClip = Resources.Load<AudioClip>("JUMP");
        jumpLandingClip = Resources.Load<AudioClip>("LAND");
        dashClip = Resources.Load<AudioClip>("DASH_SHORT1");
        timeWarpClip = Resources.Load<AudioClip>("TIME_SLOWDOWN");
        pendulumClip = Resources.Load<AudioClip>("PENDULUM");
        grapplingClip = Resources.Load<AudioClip>("HOOKSHOT");
        countDownClip = Resources.Load<AudioClip>("COUNTDOWN");
        rewindClip = Resources.Load<AudioClip>("DEATH_REWIND");
        checkpointClip = Resources.Load<AudioClip>("CHECKPOINT");
        bulletClips = new AudioClip[] {
            Resources.Load<AudioClip>("TURRET_BULLET_1"),
            Resources.Load<AudioClip>("TURRET_BULLET_2"),
            Resources.Load<AudioClip>("TURRET_BULLET_3")
        };

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
        highPitchSoundEffectsAudioSource.volume = 0.5f * newVolume;
        laserAudioSource.volume = 0.5f * newVolume;
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
        if (highPitchSoundEffectsAudioSource && jumpClip)
        {
            highPitchSoundEffectsAudioSource.PlayOneShot(jumpClip, volume);
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

    public void PlayBulletSound(Vector3 turretPosition)
    {
        if (bulletClips == null || bulletClips.Length <= 0)
        {
            return;
        }

        // Play a random bullet noise.
        int clipNumber = Random.Range(0, bulletClips.Length);
        AudioSource.PlayClipAtPoint(bulletClips[clipNumber], turretPosition, volume);
    }

    /**
     * Sets the laser AudioSource volume depending on the distance between the player and the closest laser.
     */
    private void UpdateLaserVolume()
    {
        // Sound volume is proportional to how far away the player is.
        float minLaserPlayerDistance = float.MaxValue;
        foreach (Vector3 laserPosition in lasersPlayingSound)
        {
            float playerLaserDistance = Vector3.Distance(laserPosition, playerTransform.position);
            minLaserPlayerDistance = Mathf.Min(playerLaserDistance, minLaserPlayerDistance);
        }
        float laserSoundVolume = volume * ((laserSoundThreshold - minLaserPlayerDistance) / laserSoundThreshold);
        laserAudioSource.volume = laserSoundVolume;
    }

    private void FixedUpdate()
    {
        UpdateLaserVolume();
    }

    /**
     * Only play the laser sound if this is the first laser.
     */
    public void StartLaserSound(Vector3 laserPosition)
    {
        if (lasersPlayingSound.Count == 0)
        {
            UpdateLaserVolume();
            laserAudioSource.Play();
        }
        lasersPlayingSound.Add(laserPosition);
    }

    /**
     * Only stop the laser sound if there are no more lasers left.
     */
    public void StopLaserSound(Vector3 laserPosition)
    {
        lasersPlayingSound.Remove(laserPosition);
        if (lasersPlayingSound.Count == 0)
        {
            laserAudioSource.Stop();
        }
    }
}
