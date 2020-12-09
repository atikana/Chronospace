using UnityEngine;

public class TurretBullet : MonoBehaviour
{

    public float movementSpeed = 60f;
    private float timer = 0f;
    private GameManager gameManager;
    private bool isActive;
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
        if (other.CompareTag("Player") || (!other.CompareTag("Untagged") && !other.CompareTag("Bullet") && !other.CompareTag("Turret")))
        {
            Vector3 pos = this.transform.position;
            Instantiate(hitEffect, pos, Quaternion.identity);
            isActive = false;
            Destroy(this.gameObject);

            if (other.CompareTag("Player"))
            {
                gameManager.KillPlayer();
            }
        }
    }

    /*
    private void OnCollisionEnter(Collision co)
    {   
        Collider other = co.collider;
        ContactPoint contact = co.contacts[0];
        Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if (other.CompareTag("Player"))
        {
            Debug.Log(other.tag);
            Debug.Log("hit effect");
            var hit = Instantiate(hitEffect, pos, rot);
        }
        else if (!other.CompareTag("Untagged"))
        {
            Debug.Log(other.tag);
            Debug.Log("hit effect");
            var hit = Instantiate(hitEffect, pos, rot);
        }
    }
    */
}
