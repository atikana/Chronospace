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
    private PlayerInput input;
    private GameManager gameManager;
    private SoundManager soundManager;
    private ParticleSystem cameraParticleSystem;
    private LevelStats levelStats;
    private CapsuleCollider playerCapsuleCollider;

    public float jumpForce = 550f;
    private bool firstJumpUsed = false;
    private bool secondJumpUsed = false;
    private bool grounded = true;
    private bool cancellingGrounded = false;
    public float onAirControlLR = 2f; // change this value to adjust player's ability to move left/right in mid-air
    public float onAirControlFB = 2f; // change this value to adjust player's ability to move forward/backward in mid-air

    private float movementSpeed = 4500f;

    public float maxSpeed = 30f;
    public float counterMovement = 0.175f;
    private float counterMovementThreshold = 0.01f;
    private float maxSlopeAngle = 45f;

    public LayerMask groundMask;
    public LayerMask pendulumMask;
    public LayerMask grappleMask;

    private bool dashing = false;

    private float steppingUpVelocity = 7f;
    private bool steppingUp = false;
    private float stepSize;
    private const float maxStepSize = 1.0f;

    private bool climbingPlatform = false;
    private float climbingSpeed = 15f;
    private float beforeClimbingSpeed;
    private float endClimbingSpeed = 5f;
    private float climbingAngleThreshold = 25f;
    private Vector2 climbingPlatformNormal;
    private float climbingPlatformTop;
    private float beforeClimbCounter = 0f;
    public float beforeClimbTime = 0.7f;

    // Offset between the player's head and their hands when reached above.
    private const float climbingOffset = 0.3f;

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

    private List<PositionRecord> previousPositions;
    private float rewindPeriod = 0.0f;
    public float rewindStep = 0.1f;
    public bool isRewinding;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        handsAnimator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        cameraParticleSystem = GetComponentInChildren<ParticleSystem>();
        levelStats = FindObjectOfType<LevelStats>();
        playerCapsuleCollider = GetComponent<CapsuleCollider>();
        previousPositions = new List<PositionRecord>();
        rippleCameraEffect = cameraTransform.GetComponent<RippleEffect>();
        input = new PlayerInput();
    }

    /**
     * Set up stuff before the level starts.
     */
    void Start()
    {
        isRewinding = false;

        // Reset animation triggers to prevent them running at start.
        ResetAnimations();

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

        input.Player.Jump.performed += context => Jump();

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

        numDashes = dashCapacity;

        // Initialize look vector and camera rotation to (0, 0).
        lookVector = Vector2.zero;
        cameraRotation = Vector2.zero;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public PlayerInput GetInput()
    {
        return input;
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
        Vector2 horizontalVelocity = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
        if (numDashes > 0 && horizontalVelocity.magnitude > 0.1f)
        {
            if (!dashing)
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
                // TODO:  Set the player's velocity so that they fall slowly until their hands are at the top of the platform.
                rigidBody.velocity = new Vector3(0f, beforeClimbingSpeed, 0f);
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

    private void MaintainStepUp()
    {
        if (steppingUp)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, steppingUpVelocity, rigidBody.velocity.x);

            if (playerCapsuleCollider.bounds.min.y > climbingPlatformTop)
            {
                steppingUp = false;
            }
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
            // First check if the player is on the ground.
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
                    firstJumpUsed = false;
                    secondJumpUsed = false;

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


            // Next, check if the player is climbing, since it requires knowing if the player is grounded or not.
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;

                // Player hit the side of a platform.
                if (!IsFloor(normal) && IsNotBottom(normal) && isGround)
                {
                    float playerBottom = playerCapsuleCollider.bounds.min.y;
                    float platformTop = other.collider.bounds.max.y;

                    // Handle the case where the player is trying to walk onto a slightly taller platform.
                    if (!climbingPlatform && !steppingUp && grounded && climbingPlatformTop - playerBottom < maxStepSize && rigidBody.velocity.y >= -0.01)
                    {
                        steppingUp = true;
                        climbingPlatformTop = platformTop;
                        rigidBody.velocity = new Vector3(rigidBody.velocity.x, steppingUpVelocity, rigidBody.velocity.x);
                    }
                    else if (!climbingPlatform && !steppingUp)
                    {
                        climbingPlatformTop = platformTop;

                        Vector2 playerForward = new Vector2(transform.forward.x, transform.forward.z);
                        Vector2 normal2d = new Vector2(normal.x, normal.z);
                        float playerPlatformAngle = Mathf.Abs(Vector2.Angle(playerForward, -normal2d));

                        // Start climbing if the player is not yet climbing, but facing the platform.
                        if (playerPlatformAngle < climbingAngleThreshold)
                        {
                            float playerTop = playerCapsuleCollider.bounds.max.y;

                            // Distance the player must fall until the point where their hands will be on top of the platform when reaching up.
                            float distanceToHandsOnPlatform = climbingPlatformTop - playerTop - climbingOffset;

                            // The player should not fall if their head is below the platform top.
                            if (distanceToHandsOnPlatform > 0)
                            {
                                return;
                            }

                            climbingPlatform = true;
                            climbingPlatformNormal = normal2d;
                            beforeClimbCounter = beforeClimbTime;

                            /* Set player's y velocity to the distance they need to travel until their head is at the bottom
                             * of the platform, divided by the time it takes until their hands are above their head.
                             */
                            beforeClimbingSpeed = distanceToHandsOnPlatform / beforeClimbTime;

                            handsAnimator.SetTrigger("Climbing");
                        }
                    }
                }
            }
        }
    }

    private void Jump()
    {
        if (grounded && !firstJumpUsed)
        {
            FirstJump();

        }
        else if (!secondJumpUsed)
        {
            SecondJump();
        }
    }

    private void FirstJump()
    {
        firstJumpUsed = true;
        soundManager.PlayJumpSound();
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, Mathf.Max(0f, rigidBody.velocity.y), rigidBody.velocity.z);
        rigidBody.AddForce(Vector2.up * jumpForce * 1.5f);
        rigidBody.AddForce(Vector3.up * jumpForce * 0.5f);

        //Vector3 vel = rigidBody.velocity;
        //if (rigidBody.velocity.y < 0.5f)
        //{
        //    rigidBody.velocity = new Vector3(vel.x, 0, vel.z);
        //}
        //else if (rigidBody.velocity.y > 0)
        //{
        //    rigidBody.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
        //}
    }

    private void SecondJump()
    {
        secondJumpUsed = true;
        soundManager.PlayDoubleJumpSound();
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, Mathf.Max(0f, rigidBody.velocity.y), rigidBody.velocity.z);
        rigidBody.AddForce(Vector2.up * jumpForce * 1.5f);
        rigidBody.AddForce(Vector3.up * jumpForce * 0.5f);

        //Vector3 vel = rigidBody.velocity;
        //if (rigidBody.velocity.y < 0.5f)
        //{
        //    rigidBody.velocity = new Vector3(vel.x, 0, vel.z);
        //}
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
        if (!grounded || dashing) return;

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

        if (x > 0 && xMag > maxSpeed && !dashing) x = 0;
        if (x < 0 && xMag < -maxSpeed && !dashing) x = 0;
        if (y > 0 && yMag > maxSpeed && !dashing) y = 0;
        if (y < 0 && yMag < -maxSpeed && !dashing) y = 0;

        float multiplier = 1f, multiplierV = 1f;
        if (!grounded && !dashing)
        {
           // Debug.Log("!ground");
            multiplier = onAirControlFB;
            multiplierV = onAirControlLR;
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
        MaintainStepUp();
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
            rewindPeriod += Time.deltaTime;
        }
    }

    private void Update()
    {
        if (!isRewinding)
        {
            AdjustCamera();
        }
    }

    private void Rewind()
    {
        if (previousPositions.Count > 0)
        {
            PositionRecord p = previousPositions[0];
            rigidBody.transform.position = p.position;
            cameraTransform.rotation = p.rotation;
            rigidBody.transform.rotation = p.playerRotation;
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
            PositionRecord p = previousPositions[previousPositions.Count - 2];
            while (previousPositions.Count < 200)
            {
                previousPositions.Insert(previousPositions.Count - 1, p);
            }
            Debug.Log("points stored:");
            rigidBody.isKinematic = true;
            Debug.Log(previousPositions.Count);
            isRewinding = true;
            // rewindStep = 3.0f / previousPositions.Count;
            rewindStep = 3.0f / 400;
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
        previousPositions = new List<PositionRecord>();
        if (gameManager.checkPointManager.GetClosestCheckPoint(transform.position).GetCheckPointPosition() == null)
        {
            Debug.Log("start position");
            // previousPositions.Insert(0, new Vector3(-4.3f, 7.0f, -40.5f));
            previousPositions.Insert(0, new PositionRecord(new Vector3(-4.3f, 7.0f, -40.5f), Quaternion.identity, Quaternion.identity));
        }
        else
        {
            // previousPositions.Insert(0, gameManager.checkPointManager.GetClosestCheckPoint(transform.position).GetCheckPointPosition());
            previousPositions.Insert(0, new PositionRecord(gameManager.checkPointManager.GetClosestCheckPoint(transform.position).GetCheckPointPosition(), Quaternion.identity, Quaternion.identity));
            Debug.Log(gameManager.checkPointManager.GetClosestCheckPoint(transform.position).GetCheckPointPosition());
        }
    }

    private void RecordPositions()
    {
        int partition = 20;
        int i = 1;
        if (previousPositions.Count >= 401)
        {
            /***
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
            ***/
            previousPositions.RemoveAt(200);
            previousPositions.Insert(0, new PositionRecord(rigidBody.transform.position, cameraTransform.rotation, rigidBody.transform.rotation));

        }
        else if (previousPositions.Count > 0)
        {
            if (rigidBody.transform.position != previousPositions[0].position)
            {
                //Debug.Log("record position");

                // previousPositions.Insert(0, rigidBody.transform.position);
                previousPositions.Insert(0, new PositionRecord(rigidBody.transform.position, cameraTransform.rotation, rigidBody.transform.rotation));
            }
        }
        else
        {
            // previousPositions.Insert(0, new Vector3(-4.3f, 7.0f, -40.5f));
            previousPositions.Insert(0, new PositionRecord(new Vector3(-4.3f, 7.0f, -40.5f), Quaternion.identity, Quaternion.identity));
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
