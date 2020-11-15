using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    
    public void LoadLvl1()
    {
        SceneManager.LoadScene("Level1");
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

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
