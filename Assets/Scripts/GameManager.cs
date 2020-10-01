using UnityEngine;

public class GameManager : MonoBehaviour
{
    /* The speed multiplier of the moving objects in the game.
     * This allows the time warp effect to take place.
     */
    private static float gameSpeedMultiplier = 1f;

    // True if time warp is enabled. False otherwise.
    private static bool timeWarpEnabled = false;

    public static bool getTimeWarpEnabled()
    {
        return timeWarpEnabled;
    }

    public static float GetGameSpeedMultiplier()
    {
        return gameSpeedMultiplier;
    }

    /**
     * Change the game speed multiplier.
     */
    public static void SetTimeWarp(float newValue = 0.5f)
    {
        timeWarpEnabled = true;
        gameSpeedMultiplier = newValue;
    }

    /**
     * Reset the game speed multiplier.
     */
    public static void RemoveTimeWarp()
    {
        timeWarpEnabled = false;
        gameSpeedMultiplier = 1.0f;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
