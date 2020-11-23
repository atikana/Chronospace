using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionMenu : MonoBehaviour
{
    private GameManager gameManager;
    private SoundManager soundManager;
    private GameSettings gameSettings;


    Slider volume;
    Slider sensitivity;
    Toggle autoaim;

    void Awake()
    {
      

    }

    private void Start()
    {

        gameSettings = FindObjectOfType<GameSettings>();
        volume = transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        sensitivity = transform.GetChild(3).GetChild(0).GetComponent<Slider>();
        autoaim = transform.GetChild(4).GetComponent<Toggle>();

        if (string.Compare(SceneManager.GetActiveScene().name, "StartMenu") != 0)
        {
            

            gameManager = FindObjectOfType<GameManager>();
            soundManager = FindObjectOfType<SoundManager>();


        }



        if (gameSettings.CheckStartMenu())
        {

            float temp = gameSettings.GetVolume();
            volume.value = temp;

            temp = gameSettings.GetMouseSensitivity();
            sensitivity.value = temp;

            bool b = gameSettings.GetAutoAim();
            autoaim.isOn = b;
        }
        else
        {
            gameSettings.SetGameSettings();
        }
    }


    public void SetVolume()
    {
        gameSettings.SetVolume(volume.value);
        Debug.Log(volume.value);
        if (soundManager != null)
        {
            soundManager.SetVolume(volume.value);
        }
    }

    public void SetMouseSensitivity()
    {
        gameSettings.SetSensitivity(sensitivity.value);
        Debug.Log(sensitivity.value);
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

}
