using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{

    public float movementSpeed = 36f;
    public float force = 100f;
    public float timer = 0f;
    GameManager gameManager;

    PlayerControl playerControl;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerControl = FindObjectOfType<PlayerControl>();
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.fixedDeltaTime * (movementSpeed * gameManager.GetTimeWarpMultiplier()));
        timer += 1.0F * Time.deltaTime;
        if (timer >= 3)
        {
            // GameObject.Destroy(gameObject);
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameManager.KillPlayer();
        }
        else {
            Destroy(this.gameObject);
        }
    }

}
