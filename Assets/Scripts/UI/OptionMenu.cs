using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    void Awake()
    {
      

    }

    private void Start()
    {

        gameSettings = FindObjectOfType<GameSettings>();
        volume = transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        music = transform.GetChild(3).GetChild(0).GetComponent<Slider>();
        autoaim = transform.GetChild(4).GetComponent<Toggle>();
        sensitivity = transform.GetChild(5).GetChild(0).GetComponent<Slider>();

        if (string.Compare(SceneManager.GetActiveScene().name, "StartMenu") != 0)
        {
            

            gameManager = FindObjectOfType<GameManager>();
            soundManager = FindObjectOfType<SoundManager>();
            musicManager = FindObjectOfType<MusicManager>();

        }
        else
        {
            startMenuMusic = FindObjectOfType<StartMenuMusic>();
        }



        if (gameSettings.CheckStartMenu())
        {

         
            volume.value = gameSettings.GetVolume();
            sensitivity.value = gameSettings.GetMouseSensitivity();
            autoaim.isOn = gameSettings.GetAutoAim();
            music.value = gameSettings.GetMusic();
        }
        else
        {
            gameSettings.SetGameSettings();
        }
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

}
