using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource soundEffectsAudioSource;
    private AudioClip doubleJumpClip, jumpClip, dashClip, timeWarpClip, pendulumClip, grapplingClip;

    void Start()
    {
        soundEffectsAudioSource = GetComponent<AudioSource>();

        jumpClip = Resources.Load<AudioClip>("JUMP_2");
        doubleJumpClip = Resources.Load<AudioClip>("JUMP_1");
        dashClip = Resources.Load<AudioClip>("DASH_FORWARD");
        timeWarpClip = Resources.Load<AudioClip>("TIME_SLOWDOWN");
        pendulumClip = Resources.Load<AudioClip>("PENDULUM");
        grapplingClip = Resources.Load<AudioClip>("HOOKSHOT");
    }

    public void PlayJumpSound()
    {
        if (soundEffectsAudioSource && jumpClip)
        {
            soundEffectsAudioSource.PlayOneShot(jumpClip);
        }
    }

    public void PlayDoubleJumpSound()
    {
        if (soundEffectsAudioSource && doubleJumpClip)
        {
            soundEffectsAudioSource.PlayOneShot(doubleJumpClip);
        }
    }

    public void PlayDashSound()
    {
        if (soundEffectsAudioSource && dashClip)
        {
            soundEffectsAudioSource.PlayOneShot(dashClip);
        }
    }

    public void PlayTimeWarpSound()
    {
        if (soundEffectsAudioSource && timeWarpClip)
        {
            soundEffectsAudioSource.PlayOneShot(timeWarpClip);
        }
    }

    public void PlayGrapplingSound()
    {
        if (soundEffectsAudioSource && grapplingClip)
        {
            soundEffectsAudioSource.PlayOneShot(grapplingClip);
        }
    }

    public void PlayPendulumSound(Vector3 pendulumLocation)
    {
        if (pendulumClip)
        {
            /* Use PlayClipAtPoint to make pendulum noise sound
             * like it's originating at the pendulum's location.
             */
            AudioSource.PlayClipAtPoint(pendulumClip, pendulumLocation);
        }
    }
}
