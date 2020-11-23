using UnityEngine;

public class CameraFov : MonoBehaviour
{

    public float dashFov;
    public float dashFovSpeed;
    private Camera playerCamera;
    private float targetFov;
    private float fov;
    private PlayerControl playerControl;

    bool dash;

    private void Awake() {
        playerCamera = GetComponent<Camera>();
        playerControl = transform.parent.GetComponent<PlayerControl>();
        targetFov = playerCamera.fieldOfView;
        fov = targetFov;
    }

    private void Update()
    {
        float fovSpeed = 4f;

        if (playerControl.GetDashingStatus() && !dash)
        {
            dash = true;
            fov = Mathf.Lerp(fov, dashFov, Time.deltaTime * dashFovSpeed);
        }
        else
        {
            fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
            dash = false;
         
        }

        playerCamera.fieldOfView = fov;
    }

    public void SetCameraFov(float targetFov) {
        this.targetFov = targetFov;
    }
}
