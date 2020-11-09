using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetClosestCheckPoint(Vector3 pos)
    {
        // find the closest checkpoint
        float distance = 99999;
        int index = 0;
        

        for (int i = 0; i < transform.childCount; i++)
        {
           
            Transform t = transform.GetChild(i);
            float temp = Vector3.Distance(pos, t.position);
  
            if (distance > temp)
            {
                distance = temp;
                index = i;
            }
        }

       

        return transform.GetChild(index).position;
    }
}
