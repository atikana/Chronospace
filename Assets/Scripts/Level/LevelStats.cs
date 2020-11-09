using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelStats : MonoBehaviour
{
    Text levelTimerText;
    Text numDeathsText;
    Text timeWarpText;

    private float timer;
    public PauseMenu pauseMenu;

    public PlayerControl playerControl;

    private void Awake()
    {
        levelTimerText = transform.GetChild(0).GetComponent<Text>();
        numDeathsText = transform.GetChild(1).GetComponent<Text>();
        timeWarpText = transform.GetChild(2).GetComponent<Text>();
    }

    void Start()
    {
        ResetTimer();
    }

    public void ResetTimer()
    {
        timer = 0f;
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

        timeWarpText.text = "Time Warps:  " + playerControl.GetNumTimeWarps();
    }
}
