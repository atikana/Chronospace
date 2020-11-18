using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public LevelStats levelStats;
    public CheckPointManager checkPointManager;
    private PlayerControl playerControl;
    private GrapplingGun grapplingGun;
    public Rigidbody playerRigidbody;
    public Transform cameraTransform;
    private float deathDelay = 0f;
    private bool delayPeriod;
    public int countdown;
    public Text countdownDisplay;
    public Text rewindDisplay;
    private bool counted;
    private bool isRewinding;
    public bool RewindEnabled;

    /* The speed multiplier of the moving objects in the game.
     * This allows the time warp effect to take place.
     */
    private const float timeWarpMultiplier = 0.5f;

    private bool timeWarpEnabled;
    private bool autoAim;

    private int numPlayerDeaths = 0;

    // Look joystick/mouse sensitivity.
    private float sensitivity = 7;

    private const float baseSensitivity = 50f;
    private const float sensitivityMultiplier = 8f;


    void Start()
    {
        playerControl = FindObjectOfType<PlayerControl>();
        grapplingGun = playerControl.gameObject.GetComponentInChildren<GrapplingGun>();
        counted = false;
        isRewinding = false;
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

            playerControl.ResetPositions();
            // Debug.Log("level restarted");
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

            // Reset grappling gun.
            grapplingGun.StopGrapple();

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
        if (!isRewinding)
        {
            if (!delayPeriod)
            {
                deathDelay = 0.5f;
                delayPeriod = true;
                if (RewindEnabled)
                {
                    StartCoroutine(Rewinding());
                }
                else 
                {
                    AddDeath();
                    RestartLevel(true);
                }
            }
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

        if (!counted) 
        {
            StartCoroutine(CountdownTo());
        }
    }

    IEnumerator Rewinding()
    {
        FindObjectOfType<SoundManager>().PlayRewindSound();
        levelStats.PauseTimer();
        isRewinding = true;
        rewindDisplay.gameObject.SetActive(true);
        playerControl.input.Disable();
        playerControl.StartRewind();

        yield return new WaitForSeconds(3f);

        playerControl.StopRewind();
        playerControl.input.Enable();
        rewindDisplay.gameObject.SetActive(false);
        levelStats.StartTimer();
        AddDeath();
        RestartLevel(true);
        isRewinding = false;
    }

    IEnumerator CountdownTo()
    {
        FindObjectOfType<SoundManager>().PlayCountdownSound();
        counted = true;
        int countdown_;
        countdown_ = countdown;
        countdownDisplay.gameObject.SetActive(true);
        playerControl.input.Disable();
        cameraTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        while (countdown_ > 0)
        {
            countdownDisplay.text = countdown_.ToString();

            yield return new WaitForSecondsRealtime(1f);

            countdown_--;
        }
        countdownDisplay.text = "START";

        yield return new WaitForSecondsRealtime(0.2f);

        playerControl.input.Enable();
        levelStats.StartTimer();
        FindObjectOfType<MusicManager>().StartMusic();
        countdownDisplay.gameObject.SetActive(false);
    }

    public void SetAutoAim()
    {
        if (autoAim)
        {
            autoAim = false;
        }
        else
        {
            autoAim = true;
        }
       
    }

    public bool GetAutoAimStatus()
    {
        return autoAim;
    }
}

