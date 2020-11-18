using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{

    RawImage crosshair;
    Color original;

    void Awake()
    {
        crosshair = GetComponent<RawImage>();
        original = crosshair.color;
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
