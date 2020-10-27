using UnityEngine;
using UnityEngine.UI;

public class LevelStats : MonoBehaviour
{
    public Text levelTimerText;
    public Text numDeathsText;
    public Text timeWarpText;
    public Text dashText;

    private float timer;
    private PauseMenu pauseMenu;

    private PlayerControl playerControl;

    void Start()
    {
        pauseMenu = FindObjectOfType<PauseMenu>();
        playerControl = FindObjectOfType<PlayerControl>();
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
            levelTimerText.text = timer.ToString();
        }

        // Update the number of deaths text.
        // TODO:  Fix this!
        //numDeathsText.text = "Deaths:  " + playerControl.GetNumDeaths();

        timeWarpText.text = "Time Warps:  " + playerControl.GetNumTimeWarps();

        dashText.text = "Dashes:  " + playerControl.GetNumDashes();
    }
}
