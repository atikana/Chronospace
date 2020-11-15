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
        RevertColourAllCheckPoints();
    }

    public CheckPoint GetClosestCheckPoint()
    {
     return lastCheckpoint;

    }

    void RevertColourAllCheckPoints()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            CheckPoint checkPoint = transform.GetChild(i).GetComponent<CheckPoint>();
            
            if (checkPoint.name.CompareTo(lastCheckpoint.name) == 0)
            {
                continue;
            }

            checkPoint.RevertColor();
        }
    }
}
