using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float volume = 0.5f;
    private AudioSource soundEffectsAudioSource, highPitchSoundEffectsAudioSource,
        laserAudioSource, menuSwitchingAudioSource;
    private AudioClip jumpClip, jumpLandingClip, dashClip, timeWarpClip, pendulumClip,
        grapplingClip, countDownClip, rewindClip, checkpointClip, deathClip, menuSwitchingClip;
    private AudioClip[] bulletClips;

    // True only when we just switched to a different menu, so that we don't play the sound.
    private bool changedMenus;

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
        playerTransform = FindObjectOfType<PlayerControl>().transform;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length > 0)
        {
            menuSwitchingAudioSource = audioSources[0];
        }
        if (audioSources.Length > 1)
        {
            soundEffectsAudioSource = audioSources[1];
        }
        if (audioSources.Length > 2)
        {
            highPitchSoundEffectsAudioSource = audioSources[2];
        }
        if (audioSources.Length > 3)
        {
            laserAudioSource = audioSources[3];
        }
    }

    void Start()
    {
        jumpClip = Resources.Load<AudioClip>("SoundEffects/JUMP");
        jumpLandingClip = Resources.Load<AudioClip>("SoundEffects/LAND");
        dashClip = Resources.Load<AudioClip>("SoundEffects/DASH_SHORT1");
        timeWarpClip = Resources.Load<AudioClip>("SoundEffects/TIME_SLOWDOWN");
        pendulumClip = Resources.Load<AudioClip>("SoundEffects/PENDULUM");
        grapplingClip = Resources.Load<AudioClip>("SoundEffects/HOOKSHOT");
        countDownClip = Resources.Load<AudioClip>("SoundEffects/COUNTDOWN");
        rewindClip = Resources.Load<AudioClip>("SoundEffects/DEATH_REWIND");
        checkpointClip = Resources.Load<AudioClip>("SoundEffects/CHECKPOINT");
        deathClip = Resources.Load<AudioClip>("SoundEffects/DEATH_HIT");
        menuSwitchingClip = Resources.Load<AudioClip>("SoundEffects/HOVERCLICK_1");
        bulletClips = new AudioClip[] {
            Resources.Load<AudioClip>("SoundEffects/TURRET_BULLET_1"),
            Resources.Load<AudioClip>("SoundEffects/TURRET_BULLET_2"),
            Resources.Load<AudioClip>("SoundEffects/TURRET_BULLET_3")
        };
    }

    public void SetHighPassFilterEnabled(bool enabled)
    {
        if (highPassFilter)
        {
            highPassFilter.enabled = enabled;
        }
    }

    public void SetLowPassFilterEnabled(bool enabled)
    {
        if (lowPassFilter)
        {
            lowPassFilter.enabled = enabled;
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;

        // Manually set the music audioSource volume.
        if (soundEffectsAudioSource)
        {
            soundEffectsAudioSource.volume = newVolume;
        }
        if (highPitchSoundEffectsAudioSource)
        {
            highPitchSoundEffectsAudioSource.volume = newVolume;
        }
        if (laserAudioSource)
        {
            laserAudioSource.volume = newVolume;
        }
        if (menuSwitchingAudioSource)
        {
            menuSwitchingAudioSource.volume = newVolume;
        }
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
            soundEffectsAudioSource.PlayOneShot(rewindClip, volume);
        }
    }

    public void PlayCheckpointSound()
    {
        if (soundEffectsAudioSource && checkpointClip)
        {
            soundEffectsAudioSource.PlayOneShot(checkpointClip, volume);
        }
    }

    public void PlayDeathSound()
    {
        if (soundEffectsAudioSource && deathClip)
        {
            soundEffectsAudioSource.PlayOneShot(deathClip, volume);
        }
    }

    /**
     * If you just changed menus, call this variable so that the menu switching
     * sound is not played as soon as you enter the new menu.
     */
    public void JustChangedMenus()
    {
        changedMenus = true;
    }

    public void PlayMenuSwitchingSound()
    {
        if (changedMenus)
        {
            changedMenus = false;
        }
        else if (!changedMenus && menuSwitchingAudioSource && menuSwitchingClip)
        {
            menuSwitchingAudioSource.PlayOneShot(menuSwitchingClip, volume);
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
        if (laserAudioSource == null)
        {
            return;
        }
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
