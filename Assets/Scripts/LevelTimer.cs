using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    public Text levelTimerText;
    private float timer;

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
        timer += Time.unscaledDeltaTime;
        levelTimerText.text = timer.ToString();
        
    }
}
