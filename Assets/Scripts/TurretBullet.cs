using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{

    public float movementSpeed = 36f;
    private GameObject target;
    public float force = 100f;
    GameManager gameManager;

    PlayerControl playerControl;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerControl = FindObjectOfType<PlayerControl>();
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.fixedDeltaTime * movementSpeed);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerControl.AddDeath();
            gameManager.RestartLevel();
        }
    }

}
