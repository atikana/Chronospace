using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenu : MonoBehaviour
{

    private GameObject menu;
    private GameObject pauseMenu;
    private GameObject optionMenu;
    public GameObject winScreen;

    public GameObject firstPauseMenu, firstOption;

    private PlayerControl playerControl;
    private GameManager gameManager;
    private SoundManager soundManager;

    private void Awake()
    {
        playerControl = FindObjectOfType<PlayerControl>();
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }
    void Start()
    {
        menu = transform.GetChild(0).gameObject;
        pauseMenu = transform.GetChild(0).GetChild(0).gameObject;
        optionMenu = transform.GetChild(0).GetChild(1).gameObject;
    }

    public void PressPause()
    {
        // If the win screen is active, you can't pause the game.
        if (winScreen.activeInHierarchy)
        {
            return;
        }
        if (!CheckPaused())
        {
            PauseGame();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstPauseMenu);
        }
        else
        {
            if (optionMenu.activeInHierarchy)
            {
                optionMenu.SetActive(false);
                pauseMenu.SetActive(true);
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(pauseMenu);
            }
            else
            {
                ResumeGame();
            }
        }

    }

    public bool CheckPaused()
    {
        return menu.activeInHierarchy;
    }

    public void PauseGame()
    {
        soundManager.SetLowPassFilterEnabled(true);
        menu.SetActive(true);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        playerControl.EnableUIControls();
    }

    public void ResumeGame()
    {
        soundManager.SetLowPassFilterEnabled(false);
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
        }
        menu.SetActive(false);
        // Delay();
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerControl.EnablePlayerControls();
    }

    public void BackToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

    public void GameOption()
    {
        pauseMenu.SetActive(false);
    }

    public void OptionButtonNavi()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOption);
    }

    public void BackButtonNavi()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstPauseMenu);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
    }
}
