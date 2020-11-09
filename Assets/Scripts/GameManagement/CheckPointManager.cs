using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{

    Vector3 checkpoint;

    // Start is called before the first frame update
    void Start()
    {
        checkpoint = transform.GetChild(0).position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCheckPoint(Vector3 pos)
    {
        checkpoint= pos;
    }

    public Vector3 GetClosestCheckPoint()
    {
       
       
        return checkpoint;
       
   
        
    }
}
