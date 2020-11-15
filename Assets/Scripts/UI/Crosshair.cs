using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{

    RawImage crosshair;
    Color original;
    // Start is called before the first frame update
    void Awake()
    {
        crosshair = GetComponent<RawImage>();
        original = crosshair.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCrossHairColor()
    {
        crosshair.color = Color.yellow;
    }

    public void RevertCrosshairColor()
    {
        crosshair.color = original;
    }
}
