using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player hits a level boundary, kill them.
        if (other.tag == "Player")
        {
            gameManager.KillPlayer();
        }
    }
}
