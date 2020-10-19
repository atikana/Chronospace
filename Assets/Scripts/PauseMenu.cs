using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public static bool paused = false;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                
            }
            else
            {
                PauseGame();
            }
          
        }
    }

    public void PauseGame() 
    {
        pauseMenu.SetActive(true);
        paused = true;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        paused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
        print("unpaused");
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void BackToMain()
    {
        Time.timeScale = 1f;
        paused = false;
        SceneManager.LoadScene("StartMenu");
        //
    }

    public void Resume()
    {
        ResumeGame();
    }

}
