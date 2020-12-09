using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    private PauseMenu pauseMenu;
    public GameObject menu;
    private SoundManager soundManager;
    public LevelStats levelStats;
    public CheckPointManager checkPointManager;
    private PlayerControl playerControl;
    private GrapplingGun grapplingGun;
    public Rigidbody playerRigidbody;
    public Transform cameraTransform;
    private float deathDelay = 0f;
    private bool delayPeriod;
    public int countdown;
    public GameObject countdownDisplay;
    public GameObject rewindAnim;
    public GameObject crosshair;
    private bool counted;
    private bool isRewinding;
    public bool RewindEnabled;
    public PostProcessVolume m_PostProcessVolume;
    public GameObject diedMsg;
    private float deathTime = 2.5f;

    /* The speed multiplier of the moving objects in the game.
     * This allows the time warp effect to take place.
     */
    private const float timeWarpMultiplier = 0.4f;

    private bool timeWarpEnabled;
    private bool autoAim;

    private int numPlayerDeaths = 0;

    // Look joystick/mouse sensitivity.
    private float sensitivity = 8f;

    private const float baseSensitivity = 50f;
    private const float sensitivityMultiplier = 8f;
    private int currDeathMessage;

    // Feel free to modify this list!
    private string[] deathMessages =
    {
        //"NOT TUBULAR DUDE",
        //"UNINSTALL",
        //"NOT RADICAL",
        //"WAY PAST NOT COOL"
        "GIT GUD",
        "YOU DIED",
        "BRUH",
        "AAAAAAAHHHHHHH-",
        "*SCREAMING SOUNDS*",
        "NOOOOOOO",
        "AAAAAAAAA-",
        "*SCREAMING IN CANADIAN*",
        "*SCREAMING IN FRENCH-CANADIAN*",
        "RIP",
        "F",
        "U BAD",
        "?",
    };

    private void Awake()
    {
        Time.timeScale = 1f;
        playerControl = FindObjectOfType<PlayerControl>();
        grapplingGun = playerControl.gameObject.GetComponentInChildren<GrapplingGun>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Start()
    {
        counted = false;
        isRewinding = false;
        rewindAnim.GetComponentInChildren<Animator>().ResetTrigger("New Trigger 0");
        rewindAnim.GetComponentInChildren<Animator>().ResetTrigger("New Trigger");
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

    /***
    public void AdjustTimeWarpMultiplier(float multiplier)
    {
        timeWarpMultiplier = multiplier;
    }
    ***/

    /**
     * Resets things related to the player, which should occur prior to rewinding.
     */
    private void ResetPlayer()
    {
        // Turn off time warp.
        playerControl.ResetTimeWarp();

        // Reset dash counters.
        playerControl.ResetDash();

        // Reset hands animation.
        playerControl.ResetAnimations();

        levelStats.setDeath(GetNumDeaths());
    }

    /**
     * Resets the grappling rope and turrets, and destroys bullets in the level.
     */
    private void ResetLevelObjects(bool disableTurrets = false)
    {
        grapplingGun.StopGrapple();
        grapplingGun.ResetRope();

        // Reset all turrets so they aren't locked onto the player.
        TurretControl[] turrets = FindObjectsOfType<TurretControl>();
        foreach (TurretControl turret in turrets)
        {
            if (disableTurrets)
            {
                turret.DisableTurret();
            }
            else
            {
                turret.ResetTurret();
            }
        }

        // Remove all existing bullets.
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
    }

    private void EnableTurrets()
    {
        TurretControl[] turrets = FindObjectsOfType<TurretControl>();
        foreach (TurretControl turret in turrets)
        {
            turret.EnableTurret();
        }
    }

    /*
     * Allows the player to restart the level they are currently on.
     */
    public void RestartLevel(bool dead = false)
    {
        if (dead)
        {
            CheckPoint lastCheckPoint = checkPointManager.GetClosestCheckPoint(playerControl.transform.position);

            playerControl.ResetPositions();

            playerRigidbody.MovePosition(lastCheckPoint.GetCheckPointPosition());

            // Reset the player's velocity and looking angle.
            playerRigidbody.velocity = Vector3.zero;
            playerControl.SetCameraRotation(new Vector2(lastCheckPoint.GetRespawnRotation(), 0f));

            // If rewind is enabled, these methods are called before the rewind begins.
            if (!RewindEnabled)
            {
                ResetPlayer();
                ResetLevelObjects();
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
        levelStats.PauseTimer();
        playerControl.DisableControls();
        crosshair.SetActive(false);

        AddDeath();
        ResetPlayer();

        Time.timeScale = 0.0f;
        m_PostProcessVolume.isGlobal = true;

        // Generates a random death message that is never the same twice in a row.
        currDeathMessage = (currDeathMessage + Random.Range(1, deathMessages.Length)) % deathMessages.Length;
        diedMsg.GetComponent<Text>().text = deathMessages[currDeathMessage];
        diedMsg.gameObject.SetActive(true);

        soundManager.PlayDeathSound();
        yield return new WaitForSecondsRealtime(deathTime);
        diedMsg.gameObject.SetActive(false);
        Time.timeScale = 1.0f;

        ResetLevelObjects(true);
        soundManager.PlayRewindSound();
        isRewinding = true;
        // timeWarpMultiplier = 0.001f;
        // SetTimeWarpEnabled(true);
        rewindAnim.gameObject.SetActive(true);
        // rewindAnim.GetComponentInChildren<Animator>().SetBool("PlayRewind", true);
        rewindAnim.GetComponentInChildren<Animator>().ResetTrigger("New Trigger 0");
        rewindAnim.GetComponentInChildren<Animator>().SetTrigger("New Trigger");
        playerControl.StartRewind();

        // yield return new WaitForSecondsRealtime(3f);
        yield return new WaitUntil(() => playerControl.isRewinding == false);
        // Debug.Log("rewind finished");
        // timeWarpMultiplier = 0.4f;
        // SetTimeWarpEnabled(false);
        EnableTurrets();
        m_PostProcessVolume.isGlobal = false;
        playerControl.StopRewind();
        playerControl.EnablePlayerControls();
        // rewindAnim.GetComponentInChildren<Animator>().SetBool("PlayRewind", false);
        rewindAnim.gameObject.SetActive(false);
        rewindAnim.GetComponentInChildren<Animator>().ResetTrigger("New Trigger");
        rewindAnim.GetComponentInChildren<Animator>().SetTrigger("New Trigger 0"); 
        crosshair.SetActive(true);
        levelStats.StartTimer();
        RestartLevel(true);
        isRewinding = false;
    }

    IEnumerator CountdownTo()
    {
        soundManager.PlayCountdownSound();
        counted = true;
        int countdown_;
        countdown_ = countdown;
        countdownDisplay.SetActive(true);
        countdownDisplay.GetComponent<Animator>().SetTrigger("StartCountdown");
        playerControl.DisableControls();
        cameraTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        while (countdown_ > 0)
        {
            yield return new WaitForSecondsRealtime(1f);

            countdown_--;
        }
        
        yield return new WaitForSecondsRealtime(0.2f);

        playerControl.EnablePlayerControls();
        levelStats.StartTimer();
        FindObjectOfType<MusicManager>().StartMusic();
        countdownDisplay.SetActive(false);
        menu.SetActive(true);
        OptionMenu optionMenu = menu.transform.GetChild(1).GetComponent<OptionMenu>();
        optionMenu.gameObject.SetActive(true);
        optionMenu.SetGameSettings();  
        optionMenu.gameObject.SetActive(false);
        menu.SetActive(false);
    }

    public void SetAutoAim(bool b)
    {
        autoAim = b;

       
    }

    public bool GetAutoAimStatus()
    {
        return autoAim;
    }
}

