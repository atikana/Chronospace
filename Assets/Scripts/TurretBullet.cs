using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{

    public float movementSpeed = 36f;
    private GameObject target;
    public float force = 100f;
    GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameManager.RestartLevel();
            /*print("Player hit");
            target = other.gameObject;
            target.GetComponent<Rigidbody>().AddForce(force * Vector3.forward);*/
        }
    }
}
