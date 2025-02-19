﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenu : MonoBehaviour
{

    private GameObject menu;
    private GameObject pauseMenu;
    private GameObject optionMenu;
    public GameObject winScreen;
    public GameObject optionMenuButton;

    public GameObject firstPauseMenu, firstOption;

    private PlayerControl playerControl;
    private SoundManager soundManager;
    private OptionMenu opMenu;

    private void Awake()
    {
        playerControl = FindObjectOfType<PlayerControl>();
        soundManager = FindObjectOfType<SoundManager>();
    }
    void Start()
    {
        menu = transform.GetChild(0).gameObject;
        pauseMenu = transform.GetChild(0).GetChild(0).gameObject;
        optionMenu = transform.GetChild(0).GetChild(1).gameObject;
        opMenu = optionMenu.GetComponent<OptionMenu>();
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
        }
        else
        {
            if (optionMenu.activeInHierarchy)
            {
                if (opMenu.ShowingControls())
                {
                    opMenu.ExitControllerScheme();
                }
                else
                {
                    ExitOptionsMenu();
                }
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
        soundManager.JustChangedMenus();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstPauseMenu);
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
        soundManager.JustChangedMenus();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOption);
    }

    public void ExitOptionsMenu()
    {
        soundManager.JustChangedMenus();
        optionMenu.SetActive(false);
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionMenuButton);
    }

    /** TODO I think we can remove this method. */
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
