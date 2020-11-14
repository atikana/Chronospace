using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnBecameInvisible()
    {
        Debug.Log(name + "is not visible");
    }

    private void OnBecameVisible()
    {
        Debug.Log(name + "is visible");
    }
}
