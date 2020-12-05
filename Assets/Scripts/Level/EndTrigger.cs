using UnityEngine;
using UnityEngine.EventSystems;

public class EndTrigger : MonoBehaviour
{
    public GameObject winScreen;
    private WinScreenMenu winScreenMenu;
    public GameObject HUD;
    private MusicManager musicManager;
    private SoundManager soundManager;
    private PlayerControl playerControl;
    private GameObject getOverHereText;
    private GameObject retrowaveSunIconFront;
    private GameObject retrowaveSunIconBack;

    private void Awake()
    {
        winScreenMenu = winScreen.GetComponent<WinScreenMenu>();
        musicManager = FindObjectOfType<MusicManager>();
        soundManager = FindObjectOfType<SoundManager>();
        playerControl = FindObjectOfType<PlayerControl>();
        retrowaveSunIconFront = transform.GetChild(0).gameObject;
        retrowaveSunIconBack = transform.GetChild(1).gameObject;
        getOverHereText = transform.GetChild(2).gameObject;
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
            winScreenMenu.SetupWinScreen();
            musicManager.PlayWinMusic();
            soundManager.JustChangedMenus();
            retrowaveSunIconFront.SetActive(false);
            retrowaveSunIconBack.SetActive(false);
            getOverHereText.SetActive(false);
            playerControl.EnableUIControls();
        }
    }
}
