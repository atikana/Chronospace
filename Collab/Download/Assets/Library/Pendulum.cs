using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    Quaternion _start, _end;

    private float angle = 90.0f;
    private float speed = 2.0f;

    
    private float _startTime = 0.0f;



    // Start is called before the first frame update
    void Start()
    {
        _start = PendulumRotation(angle);
        _end = PendulumRotation(-angle);
    }

    // Update is called once per frame
    void Update()
    {
        _startTime += Time.deltaTime;
        transform.rotation = Quaternion.Lerp(_start, _end, (Mathf.Sin(_startTime * speed + Mathf.PI / 2 ) + 1.0f ) / 2.0f );
    }

    void ResetTimer()
    {
        _startTime = 0.0f;
    }

    Quaternion PendulumRotation(float angle)
    {
        var pendulumRotation = transform.rotation;
        var angleZ = pendulumRotation.eulerAngles.z + angle;

        if (angleZ > 180) {
            angleZ -= 0;
        }
        else if (angleZ <= -180) {
            angleZ += 0;
        }
        pendulumRotation.eulerAngles = new Vector3(pendulumRotation.eulerAngles.x, pendulumRotation.eulerAngles.y, angleZ);
        return pendulumRotation;



    }
}
