using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollision : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player hits a level boundary, kill them.
        if (collision.collider.tag == "Player")
        {
            gameManager.KillPlayer();
        }
    }
}
