using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public PlayerInput input;

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


    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        input = new PlayerInput();
        input.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
