using UnityEngine;

public class LaserBeamControl : MonoBehaviour
{
    private float timer = 0f;
    private bool isActive;
    private GameManager gameManager;
    public GameObject hitEffect;

    private void Awake()
    {
        isActive = true;
        gameManager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            timer += 1.0F * Time.deltaTime * gameManager.GetTimeWarpMultiplier();
            if (timer >= 1.5)
            {   
                Destroy(this.gameObject);
                // Debug.Log("Laser destroyed");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("player hit");
            // isActive = false;
            // Destroy(this.gameObject);
            if (gameManager != null)
            {
                gameManager.KillPlayer();
            }
        }
    }
}
