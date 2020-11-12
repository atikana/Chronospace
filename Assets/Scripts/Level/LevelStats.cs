using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelStats : MonoBehaviour
{
    private Text levelTimerText;
    private Text numDeathsText;
    private Text timeWarpText;
    private Animator dashGaugeAnimator;

    private float timer;
    public PauseMenu pauseMenu;

    public PlayerControl playerControl;

    private void Awake()
    {
        dashGaugeAnimator = GetComponentInChildren<Animator>();
        levelTimerText = transform.GetChild(0).GetComponent<Text>();
        numDeathsText = transform.GetChild(1).GetComponent<Text>();
        timeWarpText = transform.GetChild(2).GetComponent<Text>();
    }

    void Start()
    {
        dashGaugeAnimator.ResetTrigger("Dashing");
        dashGaugeAnimator.ResetTrigger("StopDashing");
        ResetTimer();
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

    void Update()
    {
        // Update the level timer and set the text.
        if (!pauseMenu.CheckPaused())
        {
            timer += Time.unscaledDeltaTime;

            TimeSpan timeSpan = TimeSpan.FromSeconds(timer);

            levelTimerText.text = "Time: " + string.Format("{0,1:0}:{1,2:00}", timeSpan.Minutes, timeSpan.Seconds);
        }

        // Update the number of deaths text.
        // TODO:  Fix this!
        //numDeathsText.text = "Deaths:  " + playerControl.GetNumDeaths();
    }
}
