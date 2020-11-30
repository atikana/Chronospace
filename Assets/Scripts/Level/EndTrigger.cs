using UnityEngine;
using UnityEngine.EventSystems;

public class EndTrigger : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject HUD;
    public GameObject musicDisplay;
    public GameObject defaultWinScreenOption;

    void OnTriggerEnter(Collider other)
    {
        // When the player reaches the end of the level, show the win screen.
        if (other.tag == "Player")
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            HUD.SetActive(false);
            musicDisplay.SetActive(false);
            winScreen.SetActive(true);

            // Set the default option for the win screen.
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(defaultWinScreenOption);
        }
    }
}
