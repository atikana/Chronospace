using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{

    RawImage crosshair;
    Color original;
    RectTransform rt;

    void Awake()
    {
        crosshair = GetComponent<RawImage>();
        original = crosshair.color;
        rt = GetComponent<RectTransform>();
    }

    public void ChangeCrossHairColor()
    {
        crosshair.color = Color.green;
    }

    public void RevertCrosshairColor()
    {
        crosshair.color = original;
    }

    public void ChangeCrossHairSize()
    {
        ModifySize(65f);
    }

    public void RevertCrossHairSize()
    {
        ModifySize(35f);
    }

    private void ModifySize(float f)
    {
        rt.sizeDelta = new Vector2(f, f);

    }
}
