using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;

    /* Time scale from before the game was paused.  This is necessary so that if
     * the game is resumed while time warp is enabled, the time warp will continue.
     */
    private float originalTimeScale;
    private bool paused = false;

    private PlayerControl playerControl;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);

        // Set a default value for originalTimeScale.
        originalTimeScale = Time.timeScale;

        playerControl = GameObject.FindObjectOfType<PlayerControl>();
    }

    public void PressPause()
    {
        if (paused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public bool checkPaused()
    {
        return paused;
    }

    public void PauseGame() 
    {
        pauseMenu.SetActive(true);
        paused = true;
        Debug.Log("paused");
        //originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        paused = false;
        Time.timeScale = 1f;  // originalTimeScale;
        Cursor.visible = false;
        Debug.Log("unpaused");
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void BackToMain()
    {
        Time.timeScale = 1f;
        paused = false;
        SceneManager.LoadScene("StartMenu");
    }

    public void Resume()
    {
        ResumeGame();
    }
}
