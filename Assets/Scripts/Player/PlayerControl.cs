using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using WaterRippleForScreens;

public class PlayerControl : MonoBehaviour
{
    public Transform cameraTransform;
    private Rigidbody rigidBody;
    private Animator handsAnimator;
    public PlayerInput input;
    private GameManager gameManager;
    private SoundManager soundManager;
    private ParticleSystem cameraParticleSystem;
    private LevelStats levelStats;
    private CapsuleCollider playerCapsuleCollider;

    public float jumpForce = 550f;
    private bool ableToDoubleJump = false;
    private bool grounded = true;
    private bool cancellingGrounded = false;
    public float onAirControl = 2f; // change this value to adjust player's ability to move left/right in mid-air

    private float movementSpeed = 4500f;

    public float maxSpeed = 30f;
    public float counterMovement = 0.175f;
    private float counterMovementThreshold = 0.01f;
    private float maxSlopeAngle = 45f;

    public LayerMask groundMask;
    public LayerMask pendulumMask;
    public LayerMask grappleMask;

    private bool dashing = false;
    private bool jumping = false;
    private bool firstJump = true;
    private bool readyToJump = true;

    private bool climbingPlatform = false;
    private float climbingSpeed = 15f;
    private float endClimbingSpeed = 5f;
    private float climbingAngleThreshold = 25f;
    private Vector2 climbingPlatformNormal;
    private float climbingPlatformTop;
    private float beforeClimbCounter = 0f;
    public float beforeClimbTime = 0.5f;

    private float jumpCooldown = 0.25f;
    public float doubleJumpWindow = 0.25f;

    // Dash speed multiplier.
    public float dashMultiplier = 5f;

    // Number of seconds dash lasts for.
    private float dashLength = 0.2f;

    // Number of seconds that have passed since the last time warp finished.
    private float dashCooldownCounter = 0f;

    // Dash cooldown, in seconds.
    private float dashCooldownLength = 3f;

    private int dashCapacity = 2;
    private int numDashes;
    private float dashCounter = 0f;

    private Vector2 moveVector;
    private Vector2 lookVector;
    private Vector2 cameraRotation;

    // Maximum looking angle in y direction.
    private float maxYAngle = 90f;

    private bool running = false;

    // Number of seconds time warp lasts.
    private float timeWarpLength = 5f;

    // Number of seconds for time warp cooldown.
    private float timeWarpCooldownLength = 10f;

    private float timeWarpCounter = 0f;
    private float timeWarpCooldownCounter = 0f;
    private bool timeWarpAvailable = true;

    private RippleEffect rippleCameraEffect;

    private bool grappleShoot = false;
    private bool grappleToggle = false;

    // Multiplier for player's horizontal sensitivity.
    public float additionalHorizontalSensitivity = 1.3f;

    private List<Vector3> previousPositions;
    private float rewindPeriod = 0.0f;
    public float rewindStep = 0.1f;
    public bool isRewinding;

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
        levelStats = FindObjectOfType<LevelStats>();
        playerCapsuleCollider = GetComponent<CapsuleCollider>();
        previousPositions = new List<Vector3>();
        isRewinding = false;

        // Reset animation triggers to prevent them running at start.
        ResetAnimations();

        // Set up player input.
        input = new PlayerInput();
        input.Enable();

        input.Player.Move.performed += context =>
        {
            // Set animation trigger if player is starting to run.
            if (handsAnimator && moveVector == Vector2.zero && grounded)
            {
                handsAnimator.SetTrigger("StartRunning");
            }
            moveVector = context.ReadValue<Vector2>();
        };
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
        //input.Player.RestartLevel.performed += context => gameManager.RestartLevel();

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

        rippleCameraEffect = cameraTransform.GetComponent<RippleEffect>();

        numDashes = dashCapacity;

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
     * Reset variables related to time warp.
     */
    public void ResetTimeWarp()
    {
        gameManager.SetTimeWarpEnabled(false);
        soundManager.SetHighPassFilterEnabled(false);
        timeWarpAvailable = true;
        timeWarpCounter = 0f;
        timeWarpCooldownCounter = 0f;
        rippleCameraEffect.StopAllEffects();
        levelStats.StopTimeWarpGaugeAnimation();
    }

    /**
     * Called every FixedUpdate.  Updates variables related to time warp.
     */
    private void MaintainTimeWarp()
    {
        if (timeWarpCounter > 0)
        {
            timeWarpCounter -= Time.fixedUnscaledDeltaTime;
        }

        timeWarpCounter = Mathf.Max(timeWarpCounter, 0f);
        if (timeWarpCounter == 0 && gameManager.GetTimeWarpEnabled())
        {
            gameManager.SetTimeWarpEnabled(false);
            soundManager.SetHighPassFilterEnabled(false);
            timeWarpCooldownCounter = timeWarpCooldownLength;
        }

        if (timeWarpCooldownCounter > 0)
        {
            timeWarpCooldownCounter -= Time.fixedUnscaledDeltaTime;
        }

        timeWarpCooldownCounter = Mathf.Max(timeWarpCooldownCounter, 0f);
        if (timeWarpCooldownCounter == 0 && !gameManager.GetTimeWarpEnabled())
        {
            timeWarpAvailable = true;
        }
    }

    /**
     * Start a time warp.
     */
    private void TimeWarp()
    {
        // Can't time warp while time is already slowed down.
        if (timeWarpAvailable && !gameManager.GetTimeWarpEnabled())
        {
            timeWarpAvailable = false;
            handsAnimator.SetTrigger("TimeWarp");
            levelStats.StartTimeWarpGaugeAnimation();
            soundManager.PlayTimeWarpSound();
            soundManager.SetHighPassFilterEnabled(true);
            gameManager.SetTimeWarpEnabled(true);
            timeWarpCounter = timeWarpLength;
            rippleCameraEffect.SetNewRipplePosition(new Vector2(Screen.width / 2, (Screen.height / 2) + 35f));
        } 
    }

    /**
     * The player starts dashing.
     */
    private void Dash()
    {
        if (numDashes > 0)
        {
            Vector2 horizontalVelocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
            if (!dashing && horizontalVelocity.magnitude > 0.1f)
            {
                // dashVector is the player's joystick direction, relative to the world.
                Vector2 dashVectorForward = new Vector2(transform.forward.x, transform.forward.z) * moveVector.y;
                Vector2 dashVectorRight = new Vector2(transform.right.x, transform.right.z) * moveVector.x;
                Vector2 dashVector = (dashVectorForward + dashVectorRight).normalized;

                if (Vector2.Angle(horizontalVelocity, dashVector) < 90)
                {
                    // Add to the player's velocity.
                    Vector2 addedVelocity = horizontalVelocity.normalized * maxSpeed * dashMultiplier;
                    rigidBody.velocity += new Vector3(addedVelocity.x, 0, addedVelocity.y);
                }
                else
                {
                    // Change the player's direction completely.
                    rigidBody.velocity = new Vector3(dashVector.x * maxSpeed * dashMultiplier, rigidBody.velocity.y, dashVector.y * maxSpeed * dashMultiplier);
                }
            }

            dashing = true;
            numDashes--;
            dashCounter = dashLength;
            if (numDashes == 1)
            {
                levelStats.StartDashGaugeAnimation1();
            } else
            {
                levelStats.StartDashGaugeAnimation2();
            }

            // If you dash while a dash is reloading, you will lose your progress with reloading the dash.
            dashCooldownCounter = 0f;
            soundManager.PlayDashSound();
            if (cameraParticleSystem)
            {
                cameraParticleSystem.Play();
            }

            // Dashing gives you a very small upward force so that you can dash between platforms and not worry about falling through.
            float dashUpMultiplier = 0.01f;
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
        if (dashCounter == 0 && dashing)
        {
            // After the delay, stop the dash.
            dashing = false;
            rigidBody.velocity = new Vector3(rigidBody.velocity.x / dashMultiplier, rigidBody.velocity.y, rigidBody.velocity.z / dashMultiplier);
            dashCooldownCounter = dashCooldownLength;

            if (cameraParticleSystem)
            {
                cameraParticleSystem.Stop();
            }
        }

        if (dashCooldownCounter > 0)
        {
            dashCooldownCounter -= Time.fixedUnscaledDeltaTime;
        }

        dashCooldownCounter = Mathf.Max(dashCooldownCounter, 0f);
        if (dashCooldownCounter == 0 && !dashing && numDashes < dashCapacity)
        {
            numDashes = dashCapacity;
            dashCooldownCounter = dashCooldownLength;
        }

        
    }

    /**
     * Allows the player to climb up a platform.
     * To be called in the FixedUpdate method.
     */
    private void MaintainClimb()
    {
        if (!climbingPlatform)
        {
            return;
        }

        if (beforeClimbCounter > 0)
        {
            beforeClimbCounter -= Time.fixedDeltaTime;
            beforeClimbCounter = Mathf.Max(beforeClimbCounter, 0f);

            if (beforeClimbCounter > 0)
            {
                // TODO:  Set the player's velocity so that after half a second of falling, their hands are at the top of the platform.
                rigidBody.velocity = new Vector3(0f, -1f, 0f);
                return;
            }
        }

        rigidBody.velocity = new Vector3(0f, climbingSpeed, 0f);

        float playerBottom = playerCapsuleCollider.bounds.min.y;
        if (playerBottom > climbingPlatformTop)
        {
            // Finished climbing, so move forward slightly.
            climbingPlatform = false;
            rigidBody.velocity = new Vector3(climbingPlatformNormal.x, 0f, climbingPlatformNormal.y) * -endClimbingSpeed;
        }
    }

    private void OnCollisionStay(Collision other)
    {
        // Make sure we are only checking for walkable layers.
        int layer = other.gameObject.layer;
        bool isGround = groundMask == (groundMask | (1 << layer));
        bool isGrapple = grappleMask == (grappleMask | (1 << layer));
        if (isGround || isGrapple)
        {
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;
                //FLOOR
                if (IsFloor(normal))
                {
                    if (!grounded)
                    {
                        soundManager.PlayJumpLandingSound();
                    }

                    grounded = true;
                    if (!firstJump)
                    {
                        firstJump = true;
                    }

                    cancellingGrounded = false;

                    CancelInvoke(nameof(StopGrounded));
                }
                // Player hit the side of a platform.
                else if ((IsNotBottom(normal)) && isGround)
                {
                    Vector2 playerForward = new Vector2(transform.forward.x, transform.forward.z);
                    Vector2 normal2d = new Vector2(normal.x, normal.z);
                    float playerPlatformAngle = Vector2.Angle(playerForward, -normal2d);

                    // Start climbing if the player is facing the platform and moving.
                    // TODO:  Include a condition to make sure the player doesn't start too low.
                    if (!climbingPlatform && playerPlatformAngle < climbingAngleThreshold && moveVector.magnitude > 0)
                    {
                        climbingPlatform = true;
                        climbingPlatformNormal = normal2d;
                        climbingPlatformTop = other.collider.bounds.max.y;
                        beforeClimbCounter = beforeClimbTime;
                        handsAnimator.SetTrigger("Climbing");
                    }
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
        }
    }

    /**
     * Returns whether the vector represents the normal vector to a floor.
     */
    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    /**
     * Returns whether the vector represents the normal vector to the bottom of a platform.
     */
    private bool IsNotBottom(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.down, v);
        return angle >= maxSlopeAngle;
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
            rigidBody.AddForce(movementSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > counterMovementThreshold && Mathf.Abs(y) < 0.05f || (mag.y < -counterMovementThreshold && y > 0) || (mag.y > counterMovementThreshold && y < 0))
        {
            rigidBody.AddForce(movementSpeed * transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        if (Mathf.Sqrt((Mathf.Pow(rigidBody.velocity.x, 2) + Mathf.Pow(rigidBody.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rigidBody.velocity.y;
            Vector3 n = rigidBody.velocity.normalized * maxSpeed;
            rigidBody.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /**
     * Sets the camera's rotation to the specified rotation vector.
     */
    public void SetCameraRotation(Vector2 newRotation)
    {
        cameraRotation = newRotation;
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);
    }

    /**
     * Returns the camera's rotation.
     */
    public Vector2 GetCameraRotation()
    {
        return cameraRotation;
    }

    /**
     * Rotate the camera based on mouse/joystick movement.
     */
    private void AdjustCamera()
    {
        // Only rotate the camera left and right if the player isn't climbing up a platform.
        if (climbingPlatform)
        {
            lookVector = new Vector2(0f, lookVector.y);
        }

        cameraRotation.x = Mathf.Repeat(cameraRotation.x + lookVector.x * additionalHorizontalSensitivity * gameManager.GetSensitivity() * Time.deltaTime, 360);
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
        rigidBody.AddForce(Vector3.down * Time.deltaTime * 10);

        //velocity relative to where player is looking
        Vector2 mag = VelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

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
        if (!grounded && !dashing)
        {
           // Debug.Log("!ground");
            multiplier = 0.4f;
            multiplierV = onAirControl;
        }
        rigidBody.AddForce(transform.forward * y * movementSpeed * Time.deltaTime * multiplier * multiplier);
        rigidBody.AddForce(transform.right * x * movementSpeed * Time.deltaTime * multiplier * multiplierV);

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
     * Resets variables associated with dash ability.
     */
    public void ResetDash()
    {
        dashCounter = 0;
        dashCooldownCounter = 0;
        numDashes = 2;
        dashing = false;
        levelStats.ResetDashGaugeAnimation();
        if (cameraParticleSystem)
        {
            cameraParticleSystem.Stop();
        }
    }

    public void ResetAnimations()
    {
        handsAnimator.ResetTrigger("TimeWarp");
        handsAnimator.ResetTrigger("Grappling");
        handsAnimator.ResetTrigger("StopGrappling");
        handsAnimator.ResetTrigger("Climbing");
        handsAnimator.ResetTrigger("StartRunning");
        handsAnimator.SetBool("Running", false);
    }

    private void FixedUpdate()
    {
        MaintainDash();
        MaintainTimeWarp();
        MaintainClimb();
        Move();
        if (isRewinding)
        {
            // Rewind();
        }
        else
        {
            RecordPositions();
        }
        if (isRewinding)
        {
            if (rewindPeriod > rewindStep)
            {
                Rewind();
                rewindPeriod = 0;
            }
            rewindPeriod += UnityEngine.Time.deltaTime;
        }
    }

    private void Update()
    {
        AdjustCamera();
    }

    private void Rewind()
    {
        if (previousPositions.Count > 0)
        {
            rigidBody.transform.position = previousPositions[0];
            previousPositions.RemoveAt(0);
            //Debug.Log(previousPositions.Count);
        }
        else 
        { 
            isRewinding = false;
            
        }
    }

    public void StartRewind() 
    {
        Debug.Log(previousPositions[previousPositions.Count - 1]);
        if (previousPositions.Count != 0)
        {
            //Debug.Log("points stored:");
            rigidBody.isKinematic = true;
            //Debug.Log(previousPositions.Count);
            isRewinding = true;
            // rewindStep = 3.0f / previousPositions.Count;
            rewindStep = 3.0f / 200;
        }
    }

    public void StopRewind()
    {
        isRewinding = false;
        ResetPositions();
        rigidBody.isKinematic = false;
    }

    public void ResetPositions()
    {
        previousPositions = new List<Vector3>();
        if (gameManager.checkPointManager.GetClosestCheckPoint(transform.position).GetCheckPointPosition() == null)
        {
            Debug.Log("start position");
            previousPositions.Insert(0, new Vector3(-4.3f, 7.0f, -40.5f));
        }
        else
        {
            previousPositions.Insert(0, gameManager.checkPointManager.GetClosestCheckPoint(transform.position).GetCheckPointPosition());
            Debug.Log(gameManager.checkPointManager.GetClosestCheckPoint(transform.position).GetCheckPointPosition());
        }
    }

    private void RecordPositions()
    {
        int partition = 20;
        int i = 1;
        if (previousPositions.Count >= 201)
        {
            if (i <= partition)
            {
                previousPositions.RemoveAt(i * 10);
                i++;
            }
            else
            {
                i = 1;
                previousPositions.RemoveAt(i * 10);
            }
        }
        else if (previousPositions.Count > 0)
        {
            if (rigidBody.transform.position != previousPositions[0])
            {
                Debug.Log("record position");
                previousPositions.Insert(0, rigidBody.transform.position);
            }
        }
        else
        {
            previousPositions.Insert(0, new Vector3(-4.3f, 7.0f, -40.5f));
        }
    }

    public bool GetGroundStatus()
    {
        return grounded;
    }

    public bool GetGrapplingAutoAimStatus()
    {
        return gameManager.GetAutoAimStatus();
    }

    public bool GetDashingStatus()
    {
        return dashing;
    }

 
}
