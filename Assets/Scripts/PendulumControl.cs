using UnityEngine;

public class PendulumControl : MonoBehaviour
{
    Quaternion _start, _end;

    private float angle;  // 90f
    private float speed = 1.0f;
    
    private float _startTime = 0.0f;

    // Vector of extra force to apply to the player if they are colliding with this pendulum.
    private Vector3 playerForceVector;

    void Start()
    {
        angle = transform.rotation.eulerAngles.z;

        Debug.Log(transform.rotation.eulerAngles.z);
        if (angle == 90f)
        {
            _start = PendulumRotation(0);
            _end = PendulumRotation(180);
        }
        else
        {
            _start = PendulumRotation(180);
            _end = PendulumRotation(0);
        }

        playerForceVector = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        // Multiply by game speed multiplier to allow time warp effect.
        _startTime += Time.deltaTime;

        transform.rotation = Quaternion.Lerp(_start, _end, (Mathf.Sin(_startTime * speed + Mathf.PI / 2) + 1.0f) / 2.0f);

        // Update the force vector to apply to the player if they collide with this pendulum.
        playerForceVector.x = Mathf.Cos(_startTime * speed + Mathf.PI / 2);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Player")
        {
            Debug.Log("pendulum hits player");
            var magnitude = 5000;
            var force = transform.position - other.transform.position;
            force.Normalize();
            other.collider.GetComponent<Rigidbody>().AddForce(force * magnitude);
        }
    }

    public Vector3 getPlayerForceVector()
    {
        return playerForceVector;
    }

    void ResetTimer()
    {
        _startTime = 0.0f;
    }

    private Quaternion PendulumRotation(float angle)
    {
        Quaternion pendulumRotation = transform.rotation;
        float angleZ = pendulumRotation.eulerAngles.z + angle;

        /* TODO:  This code doesn't seem to do anything.  Can we get rid of it?
        if (angleZ > 180) {
            angleZ -= 0;
        }
        else if (angleZ <= -180) {
            angleZ += 0;
        }*/
        pendulumRotation.eulerAngles = new Vector3(pendulumRotation.eulerAngles.x, pendulumRotation.eulerAngles.y, angleZ);
        return pendulumRotation;
    }
}
