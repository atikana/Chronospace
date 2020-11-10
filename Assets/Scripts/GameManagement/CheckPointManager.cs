using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    private CheckPoint lastCheckpoint;

    void Start()
    {
        lastCheckpoint = transform.GetChild(0).GetComponent<CheckPoint>();
    }

    public void AddCheckPoint(CheckPoint checkPoint)
    {
        lastCheckpoint = checkPoint;
    }

    public CheckPoint GetClosestCheckPoint()
    {
        return lastCheckpoint;
    }
}
