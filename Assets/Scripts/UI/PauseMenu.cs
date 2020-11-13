using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    GameObject menu;
    GameObject pauseMenu;
    GameObject optionMenu;
 

    /* Time scale from before the game was paused.  This is necessary so that if
     * the game is resumed while time warp is enabled, the time warp will continue.
     */
    private float originalTimeScale;


    // Start is called before the first frame update

    private void Awake()
    {
        menu = transform.GetChild(0).gameObject;
        pauseMenu = transform.GetChild(0).GetChild(0).gameObject;
        optionMenu = transform.GetChild(0).GetChild(1).gameObject;
    }
    void Start()
    {   

        // Set a default value for originalTimeScale.
        originalTimeScale = Time.timeScale;
    }

    public void PressPause()
    {
        if (!menu.activeInHierarchy)
        {

            PauseGame();
        }
        else
        {
            if (optionMenu.activeInHierarchy)
            {
                optionMenu.SetActive(false);
                pauseMenu.SetActive(true);
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
        menu.SetActive(true);
        pauseMenu.SetActive(true);
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;


    }

    public void ResumeGame()
    {
        
        pauseMenu.SetActive(false);
        menu.SetActive(false);
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
