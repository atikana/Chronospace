using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamControl : MonoBehaviour
{
    private float timer = 0f;
    private bool isActive;
    private GameManager gameManager;
    public GameObject hitEffect;

    // Start is called before the first frame update
    void Start()
    {
        isActive = true;
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isActive)
        {
            timer += 1.0F * Time.deltaTime;
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
