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

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);

        // Set a default value for originalTimeScale.
        originalTimeScale = Time.timeScale;
    }

    public void PressPause()
    {
        if (paused)
        {
            // Need to resume with resume bottom, minor issue otherwise      
            // ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public bool CheckPaused()
    {
        return paused;
    }

    public void PauseGame()
    {
        if (pauseMenu == null)
        {
            return;
        }
        Debug.Log("HERE " + pauseMenu);
        pauseMenu.SetActive(true);
        paused = true;
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (pauseMenu == null)
        {
            return;
        }
        pauseMenu.SetActive(false);
        paused = false;
        Time.timeScale = originalTimeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void BackToMain()
    {
        // Time.timeScale = 1f;
        // paused = false;
        // SceneManager.LoadScene("StartMenu");
    }
}
