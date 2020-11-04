using UnityEngine;

public class PendulumControl : MonoBehaviour
{
    Quaternion _start, _end;

    private float angle;  // 90f
    private float speed = 4.0f;

    public float startAngle = 0.0f;
    private float _startTime = 0.0f;

    // TODO:  Figure out how to make the pendulums start facing left or right depending on this bool.
    public bool startsFacingRight = true;

    // Vector of extra force to apply to the player if they are colliding with this pendulum.
    private Vector3 playerForceVector;

    void Start()
    {
        angle = transform.rotation.eulerAngles.x;

        //Debug.Log(transform.rotation.eulerAngles.z);
        //if (angle == 90f)
        //{
            _start = PendulumRotation(startAngle);
            _end = PendulumRotation(startAngle + 180);
        //}
        //else
        //{
        //    _start = PendulumRotation(startAngle + 180);
        //    _end = PendulumRotation(startAngle);
        //}

        playerForceVector = new Vector3(0, 0, 0);
    }

    void FixedUpdate()
    {
        // Multiply by game speed multiplier to allow time warp effect.
        _startTime += Time.deltaTime;

        transform.rotation = Quaternion.Lerp(_start, _end, (Mathf.Sin(_startTime * speed + Mathf.PI / 2) + 1.0f) / 2.0f);

        // TODO:  Figure out what time the pendulum noise should be played during its swing, and make it not play if it is far away.

        // Update the force vector to apply to the player if they collide with this pendulum.
        playerForceVector.x = Mathf.Cos(_startTime * speed + Mathf.PI / 2);
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
        float angleX = pendulumRotation.eulerAngles.x + angle;

        pendulumRotation.eulerAngles = new Vector3(-angleX, pendulumRotation.eulerAngles.y, pendulumRotation.eulerAngles.x);
        return pendulumRotation;
    }
}
