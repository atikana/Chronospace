using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject levelSelect, aboutMenu, optionMenu;
    public GameObject firstMain, firstOption, firstAbout, firstLevel;

    public GameObject levelSelectButton, aboutMenuButton, optionMenuButton;

    PlayerInput input;


    public void LoadLvl1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadLvl2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("tutorial");
    }


    public void LoadLvl3()
    {
        SceneManager.LoadScene("Level3");
    }
    public void LoadInGameDesign()
    { 
        
    }

    public void PressBackButton()
    {
        if (levelSelect.activeInHierarchy)
        {
            gameObject.SetActive(true);
            levelSelect.SetActive(false);
            BackButtonNavi(levelSelectButton);
        }
        else if (aboutMenu.activeInHierarchy)
        {
            gameObject.SetActive(true);
            aboutMenu.SetActive(false);
            BackButtonNavi(aboutMenuButton);
        }
        else if (optionMenu.activeInHierarchy)
        {
            gameObject.SetActive(true);
            optionMenu.SetActive(false);
            BackButtonNavi(optionMenuButton);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    private void Awake()
    {
        input = new PlayerInput();
        input.Enable();
        input.UI.Enable();
    }

    void Start()
    {
        optionMenu.SetActive(true);
        optionMenu.GetComponent<OptionMenu>().SetMainMenuGameSettings();
        optionMenu.SetActive(false);
        FindObjectOfType<GameSettings>().SelectNewPlayerName();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstMain);
        SetupUIControls();
    }

    private void OnDestroy()
    {
        input.Disable();
    }

    /**
     * Sets up functionality for the B and start button in UI mode.
     */
    private void SetupUIControls()
    {
        input.UI.Cancel.performed += context =>
        {
            PressBackButton();
        };
    }

    public PlayerInput GetPlayerInput()
    {
        return input;
    }

    public void OptionButtonNavi()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOption);
    }

    public void LevelSelectButtonNavi()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstLevel);
    }

    public void AboutButtonNavi()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstAbout);
    }

    /**
     * When exiting a menu, go back to the last selection.
     */
    public void BackButtonNavi(GameObject lastSelection)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(lastSelection);
    }
}
