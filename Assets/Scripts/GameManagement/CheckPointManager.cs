using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{

    List<Vector3> checkpoint = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        checkpoint.Add(transform.GetChild(0).position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCheckPoint(Vector3 pos)
    {
        checkpoint.Add(pos);
    }

    public Vector3 GetClosestCheckPoint()
    {
       
       
            return checkpoint[checkpoint.Count - 1];
       
   
        
    }
}
