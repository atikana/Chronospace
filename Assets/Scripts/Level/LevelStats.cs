using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelStats : MonoBehaviour
{
    private Text levelTimerText;
    private Text numDeathsText;
    private Animator dashGaugeAnimator;
    private Animator timeWarpGaugeAnimator;

    private float timer;
    public PauseMenu pauseMenu;
    private bool startTimer;

    public PlayerControl playerControl;

    int deathCount = 0;

    private void Awake()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();
        dashGaugeAnimator = animators[0];
        timeWarpGaugeAnimator = animators[1];
        levelTimerText = transform.GetChild(0).GetComponent<Text>();
        numDeathsText = transform.GetChild(1).GetComponent<Text>();
    }

    void Start()
    {
        dashGaugeAnimator.ResetTrigger("Dashing");
        dashGaugeAnimator.ResetTrigger("StopDashing");
        timeWarpGaugeAnimator.ResetTrigger("TimeWarping");
        timeWarpGaugeAnimator.ResetTrigger("StopTimeWarping");

        ResetTimer();
        startTimer = false;
    }

    public void StartTimer()
    {
        startTimer = true;
    }

    public void PauseTimer()
    {
        startTimer = false;
    }

    public void ResetTimer()
    {
        timer = 0f;
    }

    public void StartDashGaugeAnimation()
    {
        dashGaugeAnimator.SetTrigger("Dashing");
    }

    public void StopDashGaugeAnimation()
    {
        dashGaugeAnimator.SetTrigger("StopDashing");
    }

    public void StartTimeWarpGaugeAnimation()
    {
        timeWarpGaugeAnimator.SetTrigger("StopTimeWarping");
    }

    public void StopTimeWarpGaugeAnimation()
    {
        timeWarpGaugeAnimator.SetTrigger("TimeWarping");
    }

    void Update()
    {
        // Update the level timer and set the text.
        if (startTimer)
        {
            if (!pauseMenu.CheckPaused())
            {
                timer += Time.unscaledDeltaTime;

                TimeSpan timeSpan = TimeSpan.FromSeconds(timer);

                levelTimerText.text = "Time: " + string.Format("{0,1:0}:{1,2:00}", timeSpan.Minutes, timeSpan.Seconds);
            }
        }
        

        // Update the number of deaths text.
        // TODO:  Fix this!
        //numDeathsText.text = "Deaths:  " + playerControl.GetNumDeaths();
    }

    public void setDeath(int i)
    {
        deathCount = i;

        numDeathsText.text = "Death Count: " + deathCount.ToString();

        if (!numDeathsText.gameObject.activeInHierarchy)
        {
            numDeathsText.gameObject.SetActive(true);
        }
    }


    
}
