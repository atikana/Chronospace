using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Keep track of the player's location.
    public Transform playerTransform;

    void FixedUpdate()
    {
        // Set the camera at the top of the player.
        this.transform.position = playerTransform.position + new Vector3(0, 1, 0);
    }
}
