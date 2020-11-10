using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    private GameManager gameManager;
    private SoundManager soundManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void SetVolume(float volume)
    {
        soundManager.SetVolume(volume);
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        gameManager.SetSensitivity(sensitivity);
    }        

}
