using UnityEngine;

public class OptionMenu : MonoBehaviour
{

    public GameObject soundManager;
    private GameManager gameManager;
    private AudioSource audioSource;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioSource = (AudioSource)soundManager.GetComponent("soundEffectsAudioSource");
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        gameManager.SetSensitivity(sensitivity);
    }        

}
