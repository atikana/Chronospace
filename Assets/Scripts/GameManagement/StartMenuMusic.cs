using UnityEngine;

public class StartMenuMusic : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetVolume(float f)
    {
        audioSource.volume = f;
    }
}
