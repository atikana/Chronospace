using UnityEngine;

public class StartMenuMusic : MonoBehaviour
{
    private AudioSource audioSource;
    private GameSettings gameSettings;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameSettings = FindObjectOfType<GameSettings>();
    }

    private void OnEnable()
    {
        SetVolume(gameSettings.GetMusic());
    }

    public void SetVolume(float f)
    {
        audioSource.volume = 0.75f * f;
    }
}
