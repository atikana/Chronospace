using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public Transform cameraTransform;
    private Rigidbody rigidBody;
    private Animator handsAnimator;
    private PlayerInput input;
    private GameManager gameManager;
    private SoundManager soundManager;
    private ParticleSystem cameraParticleSystem;

    public float jumpForce = 550f;
    private bool ableToDoubleJump = false;
    private bool grounded = true;
    private bool cancellingGrounded = false;

    private float movementSpeed;
    private float normalMovementSpeed = 4500f;
    private float dashMovementSpeed;

    public float maxSpeed = 30f;
    public float counterMovement = 0.175f;
    private float counterMovementThreshold = 0.01f;
    private float maxSlopeAngle = 45f;

    public LayerMask groundMask;
    public LayerMask pendulumMask;

    private bool dashing = false;
    private bool jumping = false;
    private bool firstJump = true;
    private bool readyToJump = true;

    private float jumpCooldown = 0.25f;
    public float doubleJumpWindow = 0.25f;

    // Dash speed multiplier.
    private float dashMultiplier = 10f;

    // Number of seconds dash lasts for.
    private float dashLength = 0.14f;

    // Number of seconds that have passed since the last time warp finished.
    private float dashCooldownCounter = 0f;

    // Dash cooldown, in seconds.
    private float dashCooldownLength = 3f;

    private bool dashAvailable = true;
    private float dashCounter = 0f;

    private Vector2 moveVector;
    private Vector2 lookVector;
    private Vector2 cameraRotation;

    // Maximum looking angle in y direction.
    private float maxYAngle = 90f;

    private bool running = false;

    private int numTimeWarps = 5;

    private bool grappleShoot = false;
    private bool grappleToggle = false;

    /**
     * Set up stuff before the level starts.
     */
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        handsAnimator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        cameraParticleSystem = GetComponentInChildren<ParticleSystem>();

        // Reset animation triggers to prevent them running at start.
        handsAnimator.ResetTrigger("TimeWarp");
        handsAnimator.ResetTrigger("Grappling");

        // Set up player input.
        input = new PlayerInput();
        input.Enable();

        input.Player.Move.performed += context => moveVector = context.ReadValue<Vector2>();
        input.Player.Move.canceled += context => moveVector = Vector2.zero;

        input.Player.Look.performed += context => lookVector = context.ReadValue<Vector2>();
        input.Player.Look.canceled += context => lookVector = Vector2.zero;

        input.Player.Jump.performed += context => jumping = true;
        input.Player.Jump.canceled += context =>
        {
            if (ableToDoubleJump)
            {
                firstJump = !firstJump;
            }
            jumping = false;
        };

        input.Player.Dash.performed += context => Dash();
        input.Player.TimeWarp.performed += context => TimeWarp();
        input.Player.RestartLevel.performed += context => gameManager.RestartLevel();

        input.Player.Pause.performed += context => gameManager.PauseGame();

        // TODO:  Instead, call a function within GrappleGun.
        input.Player.GrappleShoot.performed += context => grappleShoot = true;
        input.Player.GrappleShoot.canceled += context => grappleShoot = false;
        input.Player.GrappleToggle.performed += context => grappleToggle = true;
        input.Player.GrappleToggle.canceled += context => grappleToggle = false;

        /* Don't show user's cursor in the game, and lock the cursor to avoid going out of the game window.
         * Note that the Escape key can be used to show the cursor again (for example to stop running the game).
         */
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        dashMovementSpeed = normalMovementSpeed * dashMultiplier;

        // Initialize look vector and camera rotation to (0, 0).
        lookVector = Vector2.zero;
        cameraRotation = Vector2.zero;
    }

    /*
     * Returns whether or not the player is running.
     */
    public bool IsPlayerRunning()
    {
        return running;
    }

    /**
     * Start a time warp.
     */
    private void TimeWarp()
    {
        // Can't time warp while time is already slowed down.
        if (numTimeWarps > 0 && !gameManager.GetTimeWarpEnabled())
        {
            handsAnimator.SetTrigger("TimeWarp");
            numTimeWarps--;
            soundManager.PlayTimeWarpSound();
            gameManager.SetTimeWarp();
        }
    }

    /**
     * The player starts dashing.
     */
    private void Dash()
    {
        if (dashAvailable)
        {
            dashing = true;
            dashAvailable = false;
            dashCounter = dashLength;
            soundManager.PlayDashSound();
            movementSpeed = dashMovementSpeed;
            if (cameraParticleSystem)
            {
                cameraParticleSystem.Play();
            }

            // Dashing gives you a very small upward force so that you can dash between platforms and not worry about falling through.
            float dashUpMultiplier = 0.4f;
            rigidBody.AddForce(new Vector3(0, jumpForce * dashUpMultiplier, 0), ForceMode.Impulse);
        }
    }

    /**
     * Called every FixedUpdate.  Updates the dash counter
     * and dash cooldown counter to function.
     */
    private void MaintainDash()
    {
        if (dashCounter > 0)
        {
            dashCounter -= Time.fixedUnscaledDeltaTime;
        }

        dashCounter = Mathf.Max(dashCounter, 0f);
        if (dashCounter == 0)
        {
            // After the delay, stop the dash.
            if (dashing)
            {
                rigidBody.velocity = Vector3.zero;
            }
            dashing = false;
            movementSpeed = normalMovementSpeed;
            if (cameraParticleSystem)
            {
                cameraParticleSystem.Stop();
            }
            dashCooldownCounter = dashCooldownLength;
        }

        if (dashCooldownCounter > 0)
        {
            dashCooldownCounter -= Time.fixedUnscaledDeltaTime;
        }

        dashCooldownCounter = Mathf.Max(dashCooldownCounter, 0f);
        if (dashCooldownCounter == 0)
        {
            dashAvailable = true;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        // Make sure we are only checking for walkable layers.
        int layer = other.gameObject.layer;
        if (groundMask == (groundMask | (1 << layer)))
        {
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;
                //FLOOR
                if (IsFloor(normal))
                {
                    grounded = true;
                    if (!firstJump)
                    {
                        firstJump = true;
                    }

                    cancellingGrounded = false;

                    CancelInvoke(nameof(StopGrounded));
                }
            }

            float delay = 3f;
            if (!cancellingGrounded)
            {
                cancellingGrounded = true;
                Invoke(nameof(StopGrounded), Time.deltaTime * delay);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // If the player has exited a collision with a platform, they are no longer grounded.
        if (collision.gameObject.tag.Equals("Platform"))
        {
            grounded = false;

            // Able to double jump after you've left a platform.
            ableToDoubleJump = true;
        }
    }

    private void FirstJump()
    {
        // TODO:  Added "&& rigidBody" because FirstJump() was being called when rigidBody was null.  Figure out why this happens!
        if (grounded && rigidBody && readyToJump)
        {
            soundManager.PlayJumpSound();
            readyToJump = false;
            ableToDoubleJump = true;
            rigidBody.AddForce(Vector2.up * jumpForce * 1.5f);
            rigidBody.AddForce(Vector3.up * jumpForce * 0.5f);
            Vector3 vel = rigidBody.velocity;
            if (rigidBody.velocity.y < 0.5f)
            {
                rigidBody.velocity = new Vector3(vel.x, 0, vel.z);
            }
            else if (rigidBody.velocity.y > 0)
            {
                rigidBody.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            }

            Invoke(nameof(ResetJump), jumpCooldown);
            Invoke(nameof(ResetDoubleJump), doubleJumpWindow);
        }
        else
        {
           // Debug.Log("Jump failded. Not on ground");
        }
    }

    private void SecondJump()
    {
        // TODO:  Added "&& rigidBody" because SecondJump() was being called when rigidBody was null.  Figure out why this happens!

        if (!grounded && rigidBody && ableToDoubleJump)
        {
            ableToDoubleJump = false;
            soundManager.PlayDoubleJumpSound();
            rigidBody.AddForce(Vector2.up * jumpForce * 1.5f);
            rigidBody.AddForce(Vector3.up * jumpForce * 0.5f);


            Vector3 vel = rigidBody.velocity;
            if (rigidBody.velocity.y < 0.5f)
                rigidBody.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rigidBody.velocity.y > 0)
                rigidBody.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
        }
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }


    private void StopGrounded()
    {
        grounded = false;
    }

    private void ResetJump()
    {
        readyToJump = true;

    }

    private void ResetDoubleJump()
    {
        ableToDoubleJump = false;
    }

    /*
     * Returns the player's velocity relative to the direction they are looking.
     */
    public Vector2 VelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rigidBody.velocity.x, rigidBody.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rigidBody.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    /**
     * Balances out movement force on rigidbody with a counter force.
     */
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || dashing || jumping) return;

        if (Mathf.Abs(mag.x) > counterMovementThreshold && Mathf.Abs(x) < 0.05f || (mag.x < -counterMovementThreshold && x > 0) || (mag.x > counterMovementThreshold && x < 0))
        {
            rigidBody.AddForce(normalMovementSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > counterMovementThreshold && Mathf.Abs(y) < 0.05f || (mag.y < -counterMovementThreshold && y > 0) || (mag.y > counterMovementThreshold && y < 0))
        {
            rigidBody.AddForce(normalMovementSpeed * transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        if (Mathf.Sqrt((Mathf.Pow(rigidBody.velocity.x, 2) + Mathf.Pow(rigidBody.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rigidBody.velocity.y;
            Vector3 n = rigidBody.velocity.normalized * maxSpeed;
            rigidBody.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /**
     * Rotate the camera based on mouse/joystick movement.
     */
    private void AdjustCamera()
    {
        cameraRotation.x = Mathf.Repeat(cameraRotation.x + lookVector.x * gameManager.GetSensitivity() * Time.deltaTime, 360);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookVector.y * gameManager.GetSensitivity() * Time.deltaTime, -maxYAngle, maxYAngle);
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);

        // Rotate the player about the Y axis based on the camera's rotation.
        transform.eulerAngles = new Vector3(0, cameraRotation.x, 0);
    }

    /**
     * Move the player based on the controller/keyboard input.
     */
    private void Move()
    {
        //Extra gravity
        rigidBody.AddForce(Vector3.down * Time.fixedDeltaTime * 10);

        //velocity relative to where player is looking
        Vector2 mag = VelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        // If dash is enabled and your joystick isn't as far out as it could be, you should still go the same dash speed.
        if (dashCounter > 0)
        {
            moveVector.Normalize();
        }

        float x = moveVector.x, y = moveVector.y;

        CounterMovement(x, y, mag);

        if (jumping)
        {
            if (firstJump)
            {
                FirstJump();
            }
            else
            {
                SecondJump();
            }
        }

        if (x > 0 && xMag > maxSpeed && !dashing) x = 0;
        if (x < 0 && xMag < -maxSpeed && !dashing) x = 0;
        if (y > 0 && yMag > maxSpeed && !dashing) y = 0;
        if (y < 0 && yMag < -maxSpeed && !dashing) y = 0;

        float multiplier = 1f, multiplierV = 1f;
        if (!grounded)
        {
           // Debug.Log("!ground");
            multiplier = 0.4f;
            multiplierV = 0.4f;
        }
        rigidBody.AddForce(transform.forward * y * normalMovementSpeed * Time.fixedDeltaTime * multiplier * multiplierV);
        rigidBody.AddForce(transform.right * x * normalMovementSpeed * Time.fixedDeltaTime * multiplier * multiplierV);

        running = moveVector.magnitude > 0 && grounded;

        // Do running animation if the player is running.
        handsAnimator.SetBool("Running", running);
    }

    /* TODO:  Get rid of these! */
    public bool GetGrappleShoot()
    {
        return grappleShoot;
    }

    public bool GetGrappleToggle()
    {
        return grappleToggle;
    }

    /**
     * Return how many time warps the player has available.
     */
    public int GetNumTimeWarps()
    {
        return numTimeWarps;
    }

    private void FixedUpdate()
    {
        MaintainDash();
        Move();
        //AdjustCamera();
    }

    private void Update()
    {
        AdjustCamera();
    }
}
