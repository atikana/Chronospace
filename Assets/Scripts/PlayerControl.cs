using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rigidBody;
    private float jumpForce = 11f;
    private float doubleJumpForce = 9f;
    private float normalMovementSpeed;
    private float dashMovementSpeed;
    private float movementSpeed;
    private float maxSpeed = 20;
    private float counterMovement = 0.5f;
    private float threshold = 0.01f;
    private bool grounded = true;

    // 10 seconds before you regain a dash.
    private float timeBetweenDashes = 10f;
    private float timeSinceLastDash;

    public Animator handsAnimator;

    private PlayerInput input;
    private Vector2 moveVector;
    private Vector2 lookVector;

    // Number of seconds dash lasts for.
    private float dashLength = 0.14f;

    // Dash time taken so far.
    private float dashCounter = 0f;

    // Dash speed multiplier.
    private float dashMultiplier = 10f;

    public SoundManager soundManager;
    public float mouseSensitivity = 1.5f;
    public Transform cameraTransform;
    private Vector2 cameraRotation;
    private float maxYAngle = 90f;

    private bool ableToDoubleJump = false;

    // TODO get rid of these.
    public Animator pendulumAnimator1;
    public Animator pendulumAnimator2;
    public Animator pendulumAnimator3;
    public Animator pendulumAnimator4;
    public Animator droneAnimator1;
    public Animator droneAnimator2;
    public Animator droneAnimator3;
    public Animator droneAnimator4;

    private GameManager gameManager;
    private PauseMenu pauseMenu;

    // True if the camera should be bobbing up and down.
    private bool bobbing = false;

    private int numDeaths;
    private int numTimeWarps;
    private int numDashes;

    private bool grappleShoot = false;
    private bool grappleToggle = false;
    private bool dashing = false;

    public ParticleSystem cameraParticleSystem;

    private State state = State.Normal;

    public enum State
    {
        Normal,
        Hookshot
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        handsAnimator = FindObjectOfType<Animator>();
        handsAnimator.ResetTrigger("TimeWarp");
        handsAnimator.ResetTrigger("Grappling");

        // Set up player input.
        input = new PlayerInput();
        input.Enable();

        input.Player.Move.performed += context => moveVector = context.ReadValue<Vector2>();
        input.Player.Move.canceled += context => moveVector = Vector2.zero;

        input.Player.Look.performed += context => lookVector = context.ReadValue<Vector2>();
        input.Player.Look.canceled += context => lookVector = Vector2.zero;

        input.Player.Jump.performed += context => Jump();
        input.Player.Dash.performed += context =>
        {
            // Can't dash while currently dashing.
            if (numDashes > 0 && dashCounter == 0)
            {
                numDashes--;
                dashCounter = dashLength;
            }
        };
        input.Player.TimeWarp.performed += context =>
        {
            // Can't time warp while time is already slowed down.
            if (numTimeWarps > 0 && !gameManager.GetTimeWarpEnabled())
            {
                handsAnimator.SetTrigger("TimeWarp");
                numTimeWarps--;
                TimeWarp();
            }
            // Cheat to get you to the end of the level.
            // transform.SetPositionAndRotation(new Vector3(502, 5, 1132), transform.rotation);
        };
        input.Player.RestartLevel.performed += context => gameManager.RestartLevel();

        input.Player.Pause.performed += context => pauseMenu.PressPause();

        input.Player.GrappleShoot.performed += context =>
        {
            grappleShoot = true;
        };
        input.Player.GrappleShoot.canceled += context => grappleShoot = false;
        input.Player.GrappleToggle.performed += context => grappleToggle = true;
        input.Player.GrappleToggle.canceled += context => grappleToggle = false;

        /* Don't show user's cursor in the game, and lock the cursor to avoid going out of the game window.
         * Note that the Escape key can be used to show the cursor again (for example to stop running the game).
         */
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Camera starts out facing forward.
        cameraRotation = new Vector2(0, 0);

        rigidBody = GetComponent<Rigidbody>();

        normalMovementSpeed = 500f;
        dashMovementSpeed = normalMovementSpeed * dashMultiplier;

        timeSinceLastDash = timeBetweenDashes;

        // Initialize player movement and look vectors to (0, 0).
        lookVector = Vector2.zero;
        cameraRotation = Vector2.zero;

        numDeaths = 0;
        numDashes = 3;
        numTimeWarps = 5;
    }

    /*
     * Returns whether or not the camera should be bobbing up and down.
     */
    public bool GetBobbing()
    {
        return bobbing;
    }

    private void TimeWarp()
    {
        soundManager.PlayTimeWarpSound();
        gameManager.SetTimeWarp();
    }

    private void Dash()
    {
        if (dashCounter > 0f)
        {
            // Play dash sound at the beginning of the dash.
            if (dashCounter == dashLength)
            {
                dashing = true;
                soundManager.PlayDashSound();
                cameraParticleSystem.Play();
                timeSinceLastDash = 0;
                //float dashUpMultiplier = 0.4f;
                //float timeScaleMultiplier = 1 / Time.timeScale;
                //rigidBody.AddForce(new Vector3(0, jumpForce * dashUpMultiplier * timeScaleMultiplier * timeScaleMultiplier, 0), ForceMode.Impulse);
            }

            dashCounter -= Time.fixedUnscaledDeltaTime;
            movementSpeed = dashMovementSpeed;
        }
        else
        {
            //if (dashing)
            //{
            //    rigidBody.velocity = Vector3.zero;
            //}
            //dashing = false;
            movementSpeed = normalMovementSpeed;
            cameraParticleSystem.Stop();
        }
        dashCounter = Mathf.Clamp(dashCounter, 0f, dashLength);

        timeSinceLastDash += Time.fixedUnscaledDeltaTime;
        timeSinceLastDash = Mathf.Clamp(timeSinceLastDash, 0, timeBetweenDashes);
        if (numDashes < 3 && timeSinceLastDash == 10)
        {
            numDashes++;
            timeSinceLastDash = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*
         * If the player has entered a collision with a platform, they are grounded.
         */
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = true;
        }
        else if (collision.collider.tag.Equals("Pendulum"))
        {
            float magnitude = 1000f;
            //Vector3 force = transform.position - collision.transform.position;
            Vector3 force = collision.GetContact(0).normal;
            force.z = 0;
            force.Normalize();
            this.rigidBody.AddForce(magnitude * force);
            Debug.Log("Here:  " + magnitude * force);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // If the player has exited a collision with a platform, they are no longer grounded.
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = false;

            // Able to double jump after you've exited a collision with a platform.
            ableToDoubleJump = true;
        }
    }

    public void GrapplingHookAnimation()
    {
        handsAnimator.SetTrigger("Grappling");
    }

    private void Jump()
    {
        // Added "&& rigidBody" because Jump() was being called when rigidBody was null.
        float timeScaleMultiplier = 1 / Time.timeScale;
        if (this.grounded && rigidBody)
        {
            soundManager.PlayJumpSound();
            rigidBody.AddForce(new Vector3(0, jumpForce * timeScaleMultiplier, 0), ForceMode.Impulse);
        }
        else if (!this.grounded && ableToDoubleJump)
        {
            soundManager.PlayDoubleJumpSound();
            // TODO:  I put this if statement here because this is sometimes null.  Figure out why that is!
            if (rigidBody)
            {
                rigidBody.AddForce(new Vector3(0, doubleJumpForce * timeScaleMultiplier, 0), ForceMode.Impulse);
            }
            ableToDoubleJump = false;
        }
    }

    private void AdjustCamera()
    {
        // Rotate the camera based on mouse/joystick movement.
        cameraRotation.x = Mathf.Repeat(cameraRotation.x + lookVector.x * mouseSensitivity, 360);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookVector.y * mouseSensitivity, -maxYAngle, maxYAngle);
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);

        // Rotate the player about the Y axis based on the camera's rotation.
        this.transform.eulerAngles = new Vector3(0, cameraRotation.x, 0);
    }

    /*
     * 
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

    private void Move()
    {
        //float timeScaleMultiplier = 1 / Time.timeScale;
        //rigidBody.AddForce(Physics.gravity * timeScaleMultiplier * timeScaleMultiplier, ForceMode.Acceleration);
        //Vector2 mag = VelRelativeToLook();
        //float xMag = mag.x, yMag = mag.y;

        // If dash is enabled and your joystick isn't as far out as it could be, you should still go the same dash speed.
        if (dashCounter > 0)
        {
            moveVector.Normalize();
        }

        handsAnimator.SetBool("Running", (moveVector.magnitude > 0 && grounded));

        //Temporary fix
        float facingAngle = transform.eulerAngles.y * Mathf.PI / 180f;
        float forwardMovement = (moveVector.y * Mathf.Cos(facingAngle) - moveVector.x * Mathf.Sin(facingAngle)) * movementSpeed * Time.fixedUnscaledDeltaTime;
        float horizontalMovement = (moveVector.y * Mathf.Sin(facingAngle) + moveVector.x * Mathf.Cos(facingAngle)) * movementSpeed * Time.fixedUnscaledDeltaTime;
        this.transform.Translate(new Vector3(horizontalMovement / 25f, 0, forwardMovement / 25f), Space.World);

        //float x = moveVector.x, y = moveVector.y;

        //CounterMovement(x, y, mag);

        //if (x > 0 && xMag > maxSpeed * timeScaleMultiplier && !dashing) x = 0;
        //if (x < 0 && xMag < -maxSpeed * timeScaleMultiplier && !dashing) x = 0;
        //if (y > 0 && yMag > maxSpeed * timeScaleMultiplier && !dashing) y = 0;
        //if (y < 0 && yMag < -maxSpeed * timeScaleMultiplier && !dashing) y = 0;


        //Debug.Log(moveVector.ToString());

        //float multiplier = 5f, multiplierV = 5f;
        //if (!grounded)
        //{
        //    multiplier = 2.5f;
        //    multiplierV = 2.5f;
        //}
        //rigidBody.AddForce(transform.forward * y * movementSpeed * Time.fixedUnscaledDeltaTime * multiplier * multiplierV * timeScaleMultiplier);
        //rigidBody.AddForce(transform.right * x * movementSpeed * Time.fixedUnscaledDeltaTime * multiplier * multiplierV * timeScaleMultiplier);

        /* Bob the camera up and down if the player is moving
         * (intentionally, not being pushed) and not grounded.
         */
        bobbing = (moveVector.magnitude > 0 && grounded);
    }

    /*
     * Provides a counter force to the player's movement so that they can move slowly using addForce().
     */
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || dashing) return;

        float timeScaleMultiplier = 1 / Time.timeScale;

        // Counter movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rigidBody.AddForce(movementSpeed * transform.right * Time.fixedUnscaledDeltaTime * -mag.x * counterMovement * timeScaleMultiplier);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rigidBody.AddForce(movementSpeed * transform.forward * Time.fixedUnscaledDeltaTime * -mag.y * counterMovement * timeScaleMultiplier);
        }
    }

    //The grappling hook script will call this function to change the state
    public void ActivateHookShotState(){

        state = State.Hookshot;
                
    }

    //The grappling hook script will call this function to change the state
    public void DisableHookShotState() {
        state = State.Normal;
    }

    public bool GetGrappleShoot()
    {
        return grappleShoot;
    }

    public bool GetGrappleToggle()
    {
        return grappleToggle;
    }

    public int GetNumDeaths()
    {
        return numDeaths;
    }

    public void AddDeath()
    {
        numDeaths++;
    }

    public void HitBoundary()
    {
        // You can't die from falling off a platform if you're currently grappling.
        if (state != State.Hookshot)
        {
            AddDeath();
            gameManager.RestartLevel();
        }
    }

    public int GetNumTimeWarps()
    {
        return numTimeWarps;
    }

    public int GetNumDashes()
    {
        return numDashes;
    }

    void FixedUpdate()
    {
        Move();
        Dash();
        AdjustCamera();
    }
}
