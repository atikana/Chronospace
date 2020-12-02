using UnityEngine;
using UnityEngine.EventSystems;

public class EndTrigger : MonoBehaviour
{
    public GameObject winScreen;
    public GameObject HUD;
    private MusicManager musicManager;

    private void Awake()
    {
        musicManager = FindObjectOfType<MusicManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // When the player reaches the end of the level, show the win screen.
        if (other.tag == "Player")
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            HUD.SetActive(false);
            winScreen.SetActive(true);
            musicManager.PlayWinMusic();
        }
    }
}
