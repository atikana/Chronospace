using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public LevelStats levelStats;
    public CheckPointManager checkPointManager;
    private PlayerControl playerControl;
    public Rigidbody playerRigidbody;
    public Transform cameraTransform;
    private float deathDelay = 0f;
    private bool delayPeriod;

    /* The speed multiplier of the moving objects in the game.
     * This allows the time warp effect to take place.
     */
    private const float timeWarpMultiplier = 0.5f;

    private bool timeWarpEnabled = false;

    private int numPlayerDeaths = 0;

    // Look joystick/mouse sensitivity.
    private float sensitivity = 7;

    private const float baseSensitivity = 50f;
    private const float sensitivityMultiplier = 8f;


    void Start()
    {
        playerControl = FindObjectOfType<PlayerControl>();
    }

    public void PauseGame()
    {
        pauseMenu.PressPause();
    }

    public void SetTimeWarpEnabled(bool enabled)
    {
        timeWarpEnabled = enabled;
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
            playerRigidbody.MovePosition(lastCheckPoint.GetCheckPointPosition());

            // Reset the player's velocity and looking angle.
            playerRigidbody.velocity = Vector3.zero;
            playerControl.SetCameraRotation(new Vector2(lastCheckPoint.GetPlayerRotation(), 0f));

            // Turn off time warp.
            playerControl.ResetTimeWarp();

            // Reset dash counters.
            playerControl.ResetDash();

            // Reset hands animation.
            playerControl.ResetAnimations();


            levelStats.setDeath(GetNumDeaths());


            // Remove all existing bullets.
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
            foreach (GameObject bullet in bullets)
            {
                Destroy(bullet);
            }
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
        return baseSensitivity + sensitivityMultiplier * sensitivity;
    }

    public void KillPlayer()
    {
        // TODO:  Modify this when checkpoints are implemented!
        if (!delayPeriod)
        {
            deathDelay = 0.5f;
            delayPeriod = true;
            AddDeath();
            RestartLevel(true);
        }
    }

    void FixedUpdate()
    {
        if (delayPeriod)
        {
            if (deathDelay > 0)
            {
                deathDelay -= Time.deltaTime;
            }
            else 
            {
                delayPeriod = false;
            }
        }
    }
}
