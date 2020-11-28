using UnityEngine;
using System.Collections.Generic;

public class CheckPointManager : MonoBehaviour
{

    Dictionary<int, CheckPoint> checkpoints = new Dictionary<int, CheckPoint>();

    private void Awake()
    {
        checkpoints.Add(1,transform.GetChild(0).GetComponent<CheckPoint>());
    }

    public void AddCheckPoint(CheckPoint checkPoint)
    {
        CheckPoint last;
        int i = int.Parse(checkPoint.gameObject.name);


        if (!checkpoints.TryGetValue(i, out last))
        {
            checkpoints.Add(i, checkPoint);
        }
       
    }

    public CheckPoint GetClosestCheckPoint(Vector3 pos)
    {
        CheckPoint cp = null;

        float dist = float.MaxValue;
        foreach (var k in checkpoints.Keys)
        {
            float temp = Vector3.Distance(checkpoints[k].transform.position, pos);
            if (dist > temp)
            {
                dist = temp;
                cp = checkpoints[k];
            }
        }

     return cp;

    }

 
}
