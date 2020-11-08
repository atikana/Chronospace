using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    GameObject pauseMenu;

    /* Time scale from before the game was paused.  This is necessary so that if
     * the game is resumed while time warp is enabled, the time warp will continue.
     */
    private float originalTimeScale;
   

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = transform.GetChild(0).gameObject;
       

        // Set a default value for originalTimeScale.
        originalTimeScale = Time.timeScale;
    }

    public void PressPause()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            PauseGame();
        }
        else
        {
            
            ResumeGame();
        }
       
    }

    public bool CheckPaused()
    {
        return pauseMenu.activeInHierarchy;
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = originalTimeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

}
