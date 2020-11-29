using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OptionMenu : MonoBehaviour
{
    private GameManager gameManager;
    private SoundManager soundManager;
    private GameSettings gameSettings;
    private MusicManager musicManager;
    private StartMenuMusic startMenuMusic;


    Slider volume;
    Slider sensitivity;
    Slider music;
    Toggle autoaim;
    bool autoaimOn;
    public GameObject checkmark;

    void Awake()
    {

        gameSettings = FindObjectOfType<GameSettings>();
        volume = transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        music = transform.GetChild(3).GetChild(0).GetComponent<Slider>();
        autoaim = transform.GetChild(4).GetComponent<Toggle>();
        autoaim.isOn = gameSettings.GetAutoAim();
        sensitivity = transform.GetChild(5).GetChild(0).GetComponent<Slider>();
        autoaimOn = false;

    }

    public void SetVolume()
    {
        gameSettings.SetVolume(volume.value);
        if (soundManager != null)
        {
            soundManager.SetVolume(volume.value);
        }
    }

    public void SetMouseSensitivity()
    {
        gameSettings.SetSensitivity(sensitivity.value);
        if (gameManager != null)
        {
            gameManager.SetSensitivity(sensitivity.value);
        }

    }

    public void SetAutoAim()
    {
        bool b;

        if (autoaim.isOn)
        {
            b = true;
        }
        else
        {
            b = false;
        }

        gameSettings.SetAutoAim(b);

        if (gameManager != null)
        {
            gameManager.SetAutoAim(b);
        }
    }

    public void SetAutoAimButton()
    {
        bool b;

        if (autoaimOn)
        {
            autoaim.isOn = false;
            autoaimOn = false;
            b = false;
        }
        else
        {
            autoaim.isOn = true;
            b = true;
            autoaimOn = true;
        }

        checkmark.gameObject.SetActive(b);
        gameSettings.SetAutoAim(b);

        if (gameManager != null)
        {
            gameManager.SetAutoAim(b);
        }
    }

    public void SetMusicVolume()
    {
        gameSettings.SetMusic(music.value);
        if (musicManager != null)
        {
            musicManager.SetVolume(music.value);
        }
        else
        {
            startMenuMusic.SetVolume(music.value);
        }

    }

    public void SetGameSettings()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        musicManager = FindObjectOfType<MusicManager>();


        volume.value = gameSettings.GetVolume();
        sensitivity.value = gameSettings.GetMouseSensitivity();
        autoaim.isOn = gameSettings.GetAutoAim();
        music.value = gameSettings.GetMusic();

        soundManager.SetVolume(volume.value);
        musicManager.SetVolume(music.value);
        gameManager.SetSensitivity(sensitivity.value);
        gameManager.SetAutoAim(autoaim.isOn);
    }

    public void SetMainMenuGameSettings()
    {
       

        startMenuMusic = FindObjectOfType<StartMenuMusic>();

        volume.value = gameSettings.GetVolume();
        sensitivity.value = gameSettings.GetMouseSensitivity();
        autoaim.isOn = gameSettings.GetAutoAim();
        music.value = gameSettings.GetMusic();
        startMenuMusic.SetVolume(music.value);
    }

}
