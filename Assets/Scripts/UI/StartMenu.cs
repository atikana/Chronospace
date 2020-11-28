using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public GameObject levelSelect, aboutMenu;
    public GameObject firstMain, firstOption, firstAbout, firstLevel, closedMain;

    public GameObject optionMenu;


    public void LoadLvl1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("tutorial");
    }

    public void LoadInGameDesign()
    { 
        
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    void Start()
    {
        optionMenu.SetActive(true);
        optionMenu.GetComponent<OptionMenu>().SetMainMenuGameSettings();
        optionMenu.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstMain);
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

    public void BackButtonNavi()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstMain);
    }
}
