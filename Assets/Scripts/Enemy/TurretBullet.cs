using UnityEngine;

public class TurretBullet : MonoBehaviour
{

    public float movementSpeed = 60f;
    private float timer = 0f;
    GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.fixedDeltaTime * movementSpeed * gameManager.GetTimeWarpMultiplier());
        timer += 1.0F * Time.deltaTime;
        if (timer >= 3)
        {
            Destroy(this.gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.KillPlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
