using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    /* The speed multiplier of the moving objects in the game.
     * This allows the time warp effect to take place.
     */
    private static float timeWarpMultiplier;

    // True if time warp is enabled. False otherwise.
    //private static bool timeWarpEnabled;

    // Number of seconds time warp lasts.
    private static float timeWarpLength;

    // timeWarpLength - [Current number of seconds the time warp has been enabled].
    private static float timeWarpCounter;

    void Start()
    {
        //timeWarpEnabled = false;
        timeWarpLength = 10f;
        timeWarpCounter = 0f;
        timeWarpMultiplier = 0.5f;
    }

    /*public static bool GetTimeWarpEnabled()
    {
        return timeWarpEnabled;
    }*/

    /*public static float GetGameSpeedMultiplier()
    {
        return gameSpeedMultiplier;
    }*/

    /**
     * Change the game speed multiplier.
     */
    public static void SetTimeWarp()
    {
        //timeWarpEnabled = true;
        timeWarpCounter = timeWarpLength;
    }

    /**
     * Reset the game speed multiplier.
     */
    public static void RemoveTimeWarp()
    {
        //timeWarpEnabled = false;
        //gameSpeedMultiplier = 1.0f;
    }

    void FixedUpdate()
    {
        if (timeWarpCounter > 0)
        {
            Time.timeScale = timeWarpMultiplier;
            timeWarpCounter -= Time.fixedUnscaledDeltaTime;
        }
        else
        {
            Time.timeScale = 1f;
        }

        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        timeWarpCounter = Mathf.Clamp(timeWarpCounter, 0f, timeWarpLength);

        // Restart current level     
        if (Input.GetKeyDown(KeyCode.R))
            {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("StartMenu");
        }
        
    }

}
