using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private PlayerControl playerScript;

    // Current y-offset of camera relative to start position.
    private float cameraBobDistance = 0f;
    private float cameraBobSpeed = 0.1f;
    private bool bobCamera = false;

    private float bobbingTimeMultiplier = 15f;
    private float bobbingTime = 0f;

    void Start()
    {
        playerScript = FindObjectOfType<PlayerControl>();
    }

    void Update()
    {
        /* Set the next version of bobbingTime if the camera should be bobbing.
         * Otherwise, bobbingTime and nextBobbingTime should be zero.
         */
        float nextBobbingTime;
        if (bobCamera)
        {
            nextBobbingTime = bobbingTime + Time.unscaledDeltaTime * bobbingTimeMultiplier;
        }
        else
        {
            nextBobbingTime = bobbingTime = 0;
        }

        /* When bobbingTime is approximately a multiple of PI, the camera's vertical
         * position is around neutral, so check if bobbing should continue.
         */
        if (bobbingTime == 0f ||
            (bobbingTime <= Mathf.PI && nextBobbingTime >= Mathf.PI) ||
            (bobbingTime <= 2 * Mathf.PI && nextBobbingTime >= 2 * Mathf.PI))
        {
            bobCamera = playerScript.IsPlayerRunning();
        }

        // bobbingTime should always stay between 0 and 2 * PI.
        if (nextBobbingTime >= 2 * Mathf.PI)
        {
            nextBobbingTime -= 2 * Mathf.PI;
        }

        bobbingTime = nextBobbingTime;
        cameraBobDistance = Mathf.Sin(bobbingTime) * cameraBobSpeed;

        // Set the camera at the top of the player, and offset it vertically using cameraBobDistance.
        transform.localPosition = new Vector3(0, 1, 0);
    }
}
