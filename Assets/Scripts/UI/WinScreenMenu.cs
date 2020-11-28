using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WinScreenMenu : MonoBehaviour
{
    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void OptionButtonNavi()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackButtonNavi()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
