using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Player") 
        { 
            print("pendulum hits player");
            var magnitude = 5000;
            var force = transform.position - other.transform.position;
            force.Normalize();
            other.collider.GetComponent<Rigidbody>().AddForce(force * magnitude);
        }
    }
}
