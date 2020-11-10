using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public LevelStats levelStats;
    public CheckPointManager checkPointManager;
    public Rigidbody player;
    public Transform cameraTransform;
    private PlayerControl playerControl;

    /* The speed multiplier of the moving objects in the game.
     * This allows the time warp effect to take place.
     */
    private const float timeWarpMultiplier = 0.5f;

    // Number of seconds time warp lasts.
    private float timeWarpLength = 10f;

    // Number of remaining seconds in current time warp.
    private float timeWarpCounter = 0f;

    private bool timeWarpEnabled = false;

    private int numPlayerDeaths = 0;

    // Look joystick/mouse sensitivity.
    private float sensitivity = 7;

    private float mouseSensitivityMultiplier = 15f;


    void Start()
    {
        playerControl = FindObjectOfType<PlayerControl>();
    }

    /**
     * Change the game speed multiplier.
     */
    public void SetTimeWarp()
    {
        timeWarpEnabled = true;
        timeWarpCounter = timeWarpLength;
    }

    /**
     * Turn off time warp.
     */
    private void ResetTimeWarp()
    {
        timeWarpEnabled = false;
        timeWarpCounter = 0f;
    }

    /**
     * Called every FixedUpdate.  Updates variables related to time warp.
     */
    private void MaintainTimeWarp()
    {
        if (timeWarpCounter > 0)
        {
            timeWarpCounter -= Time.fixedUnscaledDeltaTime;
            timeWarpCounter = Mathf.Clamp(timeWarpCounter, 0f, timeWarpLength);
        }
        else
        {
            timeWarpEnabled = false;
        }
    }

    void FixedUpdate()
    {
        MaintainTimeWarp();
    }

    public void PauseGame()
    {
        pauseMenu.PressPause();
    }

    public bool GetTimeWarpEnabled()
    {
        return timeWarpEnabled;
    }

    /* Returns the game's time scale, i.e. timeWarpMultiplier if a time warp is happening,
     * 1 otherwise.  Note that we are not directly modifying Time.timeScale because
     * this messes up things relying on physics, such as jumping.
     */
    public float GetTimeWarpMultiplier()
    {
        return (timeWarpEnabled) ? timeWarpMultiplier : 1f;
    }

    /*
     * Allows the player to restart the level they are currently on.
     */
    public void RestartLevel(bool dead = false)
    {
     
        if (dead)
        {
            CheckPoint lastCheckPoint = checkPointManager.GetClosestCheckPoint();

            // camera is shaking
            player.MovePosition(lastCheckPoint.GetCheckPointPosition());

            // Reset the player's velocity and looking angle.
            player.velocity = Vector3.zero;
            playerControl.SetCameraRotation(new Vector2(lastCheckPoint.playerRotation, 0f));

            // Turn off time warp.
            ResetTimeWarp();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public int GetNumDeaths()
    {
        return numPlayerDeaths;
    }

    public void AddDeath()
    {
        numPlayerDeaths++;
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

    /**
     * Returns the look sensitivity, multiplied by a factor
     * which allows sensitivity to be in the range [1, 15].
     */
    public float GetSensitivity()
    {
        return mouseSensitivityMultiplier * sensitivity;
    }

    public void KillPlayer()
    {
        // TODO:  Modify this when checkpoints are implemented!
        AddDeath();
        RestartLevel(true);
    }
}
