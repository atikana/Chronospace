using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{

    private bool winCondition = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (winCondition)
        {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("StartMenu");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            winCondition = true;
        }
    }
}
