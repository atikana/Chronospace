using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    /* The speed multiplier of the moving objects in the game.
     * This allows the time warp effect to take place.
     */
    private float timeWarpMultiplier;

    // Number of seconds time warp lasts.
    private float timeWarpLength;

    // timeWarpLength - [Current number of seconds the time warp has been enabled].
    private float timeWarpCounter;

    private PauseMenu pauseMenu;

    private LevelStats levelStats;

    private bool timeWarpEnabled;

    void Start()
    {
        timeWarpEnabled = false;
        timeWarpLength = 10f;
        timeWarpCounter = 0f;
        timeWarpMultiplier = 0.5f;

        levelStats = FindObjectOfType<LevelStats>();

        pauseMenu = GetComponent<PauseMenu>();// GameObject.FindObjectOfType<PauseMenu>();
    }

    /**
     * Change the game speed multiplier.
     */
    public void SetTimeWarp()
    {
        timeWarpEnabled = true;
        timeWarpCounter = timeWarpLength;
    }

    void FixedUpdate()
    {
        if (timeWarpCounter > 0)
        {
            Time.timeScale = timeWarpMultiplier;
            timeWarpCounter -= Time.fixedUnscaledDeltaTime;
        }
        else if (!pauseMenu.CheckPaused())
        {
            Time.timeScale = 1f;
            timeWarpEnabled = false;
        }

        // Set fixedDeltaTime to be proportional to the time scale.
        // TODO:  Is this a good idea?
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        timeWarpCounter = Mathf.Clamp(timeWarpCounter, 0f, timeWarpLength);
    }

    public bool GetTimeWarpEnabled()
    {
        return timeWarpEnabled;
    }

    /*
     * Allows the player to restart the level they are currently on.
     * TODO:  Maybe make it bring the player back to their last checkpoint?
     */
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        levelStats.ResetTimer();
    }

}
