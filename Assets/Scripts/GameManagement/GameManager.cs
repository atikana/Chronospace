using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    private PauseMenu pauseMenu;
    public GameObject menu;
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

    /* The speed multiplier of the moving objects in the game.
     * This allows the time warp effect to take place.
     */
    private const float timeWarpMultiplier = 0.4f;

    private bool timeWarpEnabled;
    private bool autoAim;

    private int numPlayerDeaths = 0;

    // Look joystick/mouse sensitivity.
    private float sensitivity = 7;

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
        "AAAAAAAHHHHHHH",
        "*SCREAMING SOUNDS*",
        "NOOOOOOO",
        "AAAAAAAAA",
        "*SCREAMING IN CANADIAN*",
        "*SCREAMING IN CANADIAN*",
        "RIP",
        "PRESS F TO PAY RESPECTS",
        "ALT + F4"
    };

    private void Awake()
    {
        Time.timeScale = 1f;
        playerControl = FindObjectOfType<PlayerControl>();
        grapplingGun = playerControl.gameObject.GetComponentInChildren<GrapplingGun>();
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    void Start()
    {
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
            CheckPoint lastCheckPoint = checkPointManager.GetClosestCheckPoint(playerControl.transform.position);

            playerControl.ResetPositions();

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
            grapplingGun.ResetRope();

            levelStats.setDeath(GetNumDeaths());

            // Reset all turrets so they aren't locked onto the player.
            TurretControl[] turrets = FindObjectsOfType<TurretControl>();
            foreach (TurretControl turret in turrets)
            {
                turret.ResetTurret();
            }

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
        levelStats.PauseTimer();
        playerControl.GetInput().Disable();
        crosshair.SetActive(false);

        Time.timeScale = 0.0f;
        m_PostProcessVolume.isGlobal = true;

        // Generates a random death message that is never the same twice in a row.
        currDeathMessage = (currDeathMessage + Random.Range(1, deathMessages.Length)) % deathMessages.Length;
        diedMsg.GetComponent<Text>().text = deathMessages[currDeathMessage];

        diedMsg.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        diedMsg.gameObject.SetActive(false);
        Time.timeScale = 1.0f;

        FindObjectOfType<SoundManager>().PlayRewindSound();
        isRewinding = true;
        rewindAnim.gameObject.SetActive(true);
        rewindAnim.GetComponentInChildren<Animator>().SetBool("PlayRewind", true);
        playerControl.StartRewind();

        // yield return new WaitForSecondsRealtime(3f);
        yield return new WaitUntil(() => playerControl.isRewinding == false);
        Debug.Log("rewind finished");
        m_PostProcessVolume.isGlobal = false;
        playerControl.StopRewind();
        TurretControl[] turrets = FindObjectsOfType<TurretControl>();
        foreach (TurretControl turret in turrets)
        {
            turret.ResetTurret();
        }

        // Remove all existing bullets.
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
        playerControl.GetInput().Enable();
        rewindAnim.GetComponentInChildren<Animator>().SetBool("PlayRewind", false);
        rewindAnim.gameObject.SetActive(false);
        crosshair.SetActive(true);
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
        //countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.SetActive(true);
        countdownDisplay.GetComponent<Animator>().SetTrigger("StartCountdown");
        playerControl.GetInput().Disable();
        cameraTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        while (countdown_ > 0)
        {
            //countdownDisplay.text = countdown_.ToString();

            yield return new WaitForSecondsRealtime(1f);

            countdown_--;
        }
        //countdownDisplay.text = "START";

        yield return new WaitForSecondsRealtime(0.2f);

        playerControl.GetInput().Enable();
        levelStats.StartTimer();
        FindObjectOfType<MusicManager>().StartMusic();
        //countdownDisplay.gameObject.SetActive(false);
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

