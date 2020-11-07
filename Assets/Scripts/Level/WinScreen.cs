using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    private bool winCondition = false;
    public Text youWinText;

    // TODO:  Make an actual "You Win" screen.
    private float youWinTextTotalTime = 3f;
    private float youWinTextTimer = 0f;

    void Update()
    {
        if (winCondition)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //SceneManager.LoadScene("StartMenu");

            // Reset the timer to show the text.
            youWinTextTimer = youWinTextTotalTime;
        }

        youWinTextTimer -= Time.unscaledDeltaTime;
        youWinTextTimer = Mathf.Clamp(youWinTextTimer, 0, youWinTextTotalTime);
        youWinText.gameObject.SetActive(youWinTextTimer > 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            winCondition = true;
        }
    }
}
