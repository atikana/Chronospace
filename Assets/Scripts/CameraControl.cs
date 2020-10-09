using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Keep track of the player's location.
    public Transform playerTransform;

    private float cameraBobDistance = 0f;
    private float maxCameraBobDistance = 2f;
    private float cameraBobScale = 1f;

    void Update()
    {
        if (cameraBobDistance > maxCameraBobDistance)
        {
            cameraBobScale = -1f;
        }
        else if (cameraBobDistance < 0)
        {
            cameraBobScale = 1f;
        }
        cameraBobDistance += cameraBobScale;

        // Set the camera at the top of the player.
        this.transform.position = playerTransform.position + new Vector3(0, 1 + cameraBobDistance * Time.fixedDeltaTime, 0);
    }
}
