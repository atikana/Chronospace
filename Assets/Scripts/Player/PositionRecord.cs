using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRecord
{
    public Vector3 position;
    public Quaternion rotation, playerRotation;

    public PositionRecord(Vector3 p, Quaternion r, Quaternion r_)
    {
        position = p;
        rotation = r;
        playerRotation = r_;
    }
}
