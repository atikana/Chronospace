using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private GrapplingGun grapplingGun;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the player hits a level boundary, kill them.
        if (other.tag == "Player")
        {
            grapplingGun = FindObjectOfType<GrapplingGun>();
            if (!grapplingGun.isGrapping())
            {
                gameManager.KillPlayer();
            }
        }
    }
}
