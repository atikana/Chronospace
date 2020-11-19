using UnityEngine;

public class GrappleHolder : MonoBehaviour
{
    public Material normalMaterial;
    public Material grapplingMaterial;
    private bool usingNormalMaterial = true;
    private Renderer grappleRenderer;

    private void Start()
    {
        grappleRenderer = GetComponent<Renderer>();
    }

    public void ChangeColour() {
        if (usingNormalMaterial)
        {
            Debug.Log("Grappling Material");
        }
        else
        {
            Debug.Log("Normal Material");
        }
        grappleRenderer.material = (usingNormalMaterial) ? grapplingMaterial : normalMaterial;
        usingNormalMaterial = !usingNormalMaterial;
    }
}
