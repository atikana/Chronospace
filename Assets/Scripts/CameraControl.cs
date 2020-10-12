using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Keep track of the player's location.
    public Transform playerTransform;

    // Check the player's Rigidbody component to enable camera bobbing only when the player is running on the ground.
    public Rigidbody playerRb;

    private float cameraBobDistance = 0f;
    private float cameraBobScale = 0.1f;
    public PlayerControl playerScript;
    private bool bobCamera = false;

    private float bobbingTimeMultiplier = 15f;
    private float bobbingTime = 0f;

    void Start()
    {
        GameObject playerGameObject = GameObject.Find("Player");
        playerScript = playerGameObject.GetComponent<PlayerControl>();
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
            bobCamera = playerScript.GetBobbing();
        }

        // bobbingTime should always stay between 0 and 2 * PI.
        if (nextBobbingTime >= 2 * Mathf.PI)
        {
            nextBobbingTime -= 2 * Mathf.PI;
        }

        bobbingTime = nextBobbingTime;
        cameraBobDistance = Mathf.Sin(bobbingTime) * cameraBobScale;

        // Set the camera at the top of the player, and offset it vertically using cameraBobDistance.
        this.transform.position = playerTransform.position + new Vector3(0, 1 + cameraBobDistance, 0);
    }
}
