using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerControl player;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Reload from the nearest checkpoint

            gameManager = GameObject.FindObjectOfType<GameManager>();
            player = GameObject.FindObjectOfType<PlayerControl>();
            player.HitBoundary();


        }
    }
}
