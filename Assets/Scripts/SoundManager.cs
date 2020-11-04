using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource soundEffectsAudioSource;
    private AudioClip doubleJumpClip, jumpClip, dashClip, timeWarpClip, pendulumClip;

    void Start()
    {
        soundEffectsAudioSource = GetComponent<AudioSource>();

        jumpClip = Resources.Load<AudioClip>("JUMP_2");
        doubleJumpClip = Resources.Load<AudioClip>("JUMP_1");
        dashClip = Resources.Load<AudioClip>("DASH_FORWARD");
        timeWarpClip = Resources.Load<AudioClip>("TIME_SLOWDOWN");
        pendulumClip = Resources.Load<AudioClip>("PENDULUM");
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

    public void PlayPendulumSound(Vector3 pendulumLocation)
    {
        if (pendulumClip)
        {
            AudioSource.PlayClipAtPoint(pendulumClip, pendulumLocation);
        }
    }
}
