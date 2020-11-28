using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelStats : MonoBehaviour
{
    public Text levelTimerText;
    public Text numDeathsText;
    public Animator dashGaugeAnimator;
    public Animator timeWarpGaugeAnimator;

    private float timer;
    public PauseMenu pauseMenu;
    private bool startTimer;

    public PlayerControl playerControl;

    int deathCount = 0;

    void Start()
    {
        dashGaugeAnimator.ResetTrigger("Dashing1");
        dashGaugeAnimator.ResetTrigger("Dashing2");
        dashGaugeAnimator.ResetTrigger("ResetGauge");
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

    public void StartDashGaugeAnimation1()
    {
        dashGaugeAnimator.SetTrigger("Dashing1");
    }

    public void StartDashGaugeAnimation2()
    {
        dashGaugeAnimator.SetTrigger("Dashing2");
    }

    public void ResetDashGaugeAnimation()
    {
        dashGaugeAnimator.SetTrigger("ResetGauge");
    }

    public void StartTimeWarpGaugeAnimation()
    {
        timeWarpGaugeAnimator.ResetTrigger("StopTimeWarping");
        timeWarpGaugeAnimator.SetTrigger("TimeWarping");
    }

    public void StopTimeWarpGaugeAnimation()
    {
        timeWarpGaugeAnimator.ResetTrigger("TimeWarping");
        timeWarpGaugeAnimator.SetTrigger("StopTimeWarping");
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

                // Milliseconds should be between 0 and 60.
                int millisecondsTo60 = (timeSpan.Milliseconds * 60) / 1000;
                levelTimerText.text = ": " + string.Format("{0,1:0}:{1,2:00}:{2:00}", timeSpan.Minutes, timeSpan.Seconds, millisecondsTo60);
            }
        }
    }

    public void setDeath(int i)
    {
        deathCount = i;

        numDeathsText.text = ": " + deathCount.ToString();

        if (!numDeathsText.gameObject.activeInHierarchy)
        {
            numDeathsText.gameObject.SetActive(true);
        }
    }   
}