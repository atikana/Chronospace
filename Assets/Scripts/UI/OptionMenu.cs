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

    private Text titleText;
    private GameObject background;
    private GameObject backButton;
    private Slider volume;
    private Slider sensitivity;
    private Slider music;
    private GameObject volumeText, sensitivityText, musicText;
    private Toggle autoaim;
    private bool autoaimOn;
    public GameObject checkmark;
    private GameObject controllerImage;
    private GameObject displayControllerButton;
    private GameObject controllerBackButton;
    
    void Awake()
    {
        gameSettings = FindObjectOfType<GameSettings>();
        titleText = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        background = transform.GetChild(0).GetChild(1).gameObject;
        backButton = transform.GetChild(1).gameObject;
        displayControllerButton = transform.GetChild(2).gameObject;
        volume = transform.GetChild(3).GetChild(0).GetComponent<Slider>();
        volumeText = transform.GetChild(3).GetChild(1).gameObject;
        music = transform.GetChild(4).GetChild(0).GetComponent<Slider>();
        musicText = transform.GetChild(4).GetChild(1).gameObject;
        sensitivity = transform.GetChild(5).GetChild(0).GetComponent<Slider>();
        sensitivityText = transform.GetChild(5).GetChild(1).gameObject;
        autoaim = transform.GetChild(6).GetComponent<Toggle>();
        autoaim.isOn = gameSettings.GetAutoAim();
        autoaimOn = false;
        checkmark.gameObject.SetActive(gameSettings.GetAutoAim());
        controllerBackButton = transform.GetChild(7).gameObject;
        controllerImage = transform.GetChild(8).gameObject;
        soundManager = FindObjectOfType<SoundManager>();
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
        checkmark.gameObject.SetActive(autoaim.isOn);
        gameSettings.SetAutoAim(autoaim.isOn);

        if (gameManager != null)
        {
            gameManager.SetAutoAim(autoaim.isOn);
        }
    }

    /** TODO I think we can remove this method. */
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

    public bool ShowingControls()
    {
        return controllerImage.activeInHierarchy;
    }

    public void DisplayControllerScheme()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("StartMenu"))
        {
            background.SetActive(true);
        }
        titleText.text = "Controls";
        soundManager.JustChangedMenus();
        controllerBackButton.SetActive(true);
        controllerImage.SetActive(true);
        backButton.SetActive(false);
        displayControllerButton.SetActive(false);
        volume.gameObject.SetActive(false);
        volumeText.SetActive(false);
        music.gameObject.SetActive(false);
        musicText.SetActive(false);
        sensitivity.gameObject.SetActive(false);
        sensitivityText.SetActive(false);
        autoaim.gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controllerBackButton);
    }

    public void ExitControllerScheme()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("StartMenu"))
        {
            background.SetActive(false);
        }
        titleText.text = "Options";
        controllerBackButton.SetActive(false);
        controllerImage.SetActive(false);
        backButton.SetActive(true);
        displayControllerButton.SetActive(true);
        volume.gameObject.SetActive(true);
        volumeText.SetActive(true);
        music.gameObject.SetActive(true);
        musicText.SetActive(true);
        sensitivity.gameObject.SetActive(true);
        sensitivityText.SetActive(true);
        autoaim.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(displayControllerButton);
    }

    public void SetGameSettings()
    {
        gameManager = FindObjectOfType<GameManager>();
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
