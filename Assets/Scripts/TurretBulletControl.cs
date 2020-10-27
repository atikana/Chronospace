using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO:  Can we remove this class?  No GameObjects are currently using it. */
public class TurretBulletControl : MonoBehaviour
{

    public float movementSpeed = 12.0f;
    private GameObject target;
    public float force = 100f;
    // Start is called before the first frame update
    void Start()
    {

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
            print("Player hit");
            target = other.gameObject;

            //target.GetComponent<Rigidbody>().AddForce(force * Vector3.forward) = (other.transform.getPosition() - collision.transform);
            target.GetComponent<Rigidbody>().AddForce(force * Vector3.forward);
        }
    }
}
