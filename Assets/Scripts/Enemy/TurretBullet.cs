using UnityEngine;

public class TurretBullet : MonoBehaviour
{

    public float movementSpeed = 60f;
    private float timer = 0f;
    GameManager gameManager;
    private bool isActive;
    
    void Start()
    {
        isActive = true;
        gameManager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            transform.Translate(Vector3.forward * Time.fixedDeltaTime * movementSpeed * gameManager.GetTimeWarpMultiplier());
            timer += 1.0F * Time.deltaTime;
            if (timer >= 3)
            {
                Destroy(this.gameObject);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            Destroy(this.gameObject);
            gameManager.KillPlayer();
        }
        else
        {
            isActive = false;
            Destroy(this.gameObject);
        }
    }
}
