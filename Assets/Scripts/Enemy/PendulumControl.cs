using UnityEngine;

public class PendulumControl : MonoBehaviour
{
    private GameManager gameManager;
    private SoundManager soundManager;

    // The start and end rotations of the pendulum.
    private Quaternion startRotation, endRotation;

    public float rotationSpeed = 4.0f;
    private float timePassed = 0f;

    // The pendulum should rotate 180 degrees.
    private float rotationAmount = 180f;

    // Offset used for timing for playing the pendulum swing sound.
    private float pendulumSoundTimingOffset = 0.5f;

    private Transform playerTransform;

    // If player is within this distance of a pendulum, it will play a sound.
    private const float pendulumSoundThreshold = 50f;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();

        startRotation = GetPendulumRotation(0);
        endRotation = GetPendulumRotation(rotationAmount);

        // timePassed should be initialized randomly.
        timePassed = Random.Range(0f, 2 * Mathf.PI);

        playerTransform = FindObjectOfType<PlayerControl>().transform;
    }

    void FixedUpdate()
    {
        float prevTimePassed = timePassed;

        // Multiply by time warp multiplier to allow time warp effect.
        timePassed += Time.fixedUnscaledDeltaTime * rotationSpeed * gameManager.GetTimeWarpMultiplier();

        transform.rotation = Quaternion.Lerp(startRotation, endRotation, (Mathf.Sin(timePassed + Mathf.PI / 2) + 1.0f) / 2.0f);

        // Make sure timePassed never surpasses 2 * PI.
        if (timePassed > 2 * Mathf.PI)
        {
            timePassed -= 2 * Mathf.PI;
        }

        // Play the pendulum sound once per swing.
        if (Vector3.Distance(playerTransform.position, transform.position) < pendulumSoundThreshold)
        {
            if ((prevTimePassed < pendulumSoundTimingOffset && timePassed > pendulumSoundTimingOffset) ||
            (prevTimePassed < pendulumSoundTimingOffset + Mathf.PI && timePassed > pendulumSoundTimingOffset + Mathf.PI))
            {
                // Play the pendulum sound coming from below the top of the pendulum (at approximately the player's level).
                soundManager.PlayPendulumSound(transform.position - new Vector3(0, 25f, 0));
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player collides with a pendulum, they are killed.
        if (collision.gameObject.tag.Equals("Player"))
        {
            gameManager.KillPlayer();
        }
    }

    /**
     * Returns a quaternion representing the pendulum's rotation,
     * with the x-angle offset by the specified amount.
     */
    private Quaternion GetPendulumRotation(float angle)
    {
        Quaternion pendulumRotation = transform.rotation;

        pendulumRotation.eulerAngles = new Vector3(pendulumRotation.eulerAngles.x + angle,
                                                   pendulumRotation.eulerAngles.y,
                                                   pendulumRotation.eulerAngles.z);
        return pendulumRotation;
    }
}
