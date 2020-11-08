using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rigidBody;
    private float jumpForce = 550f;
    private float normalMovementSpeed = 4500;
    private float dashMovementSpeed;
    private float movementSpeed;
    public float maxSpeed = 30;
    private float counterMovement = 0.175f;
    private float threshold = 0.01f;
    private bool grounded = true;
    private bool cancellingGrounded;

    public float maxSlopeAngle = 45f;

    public LayerMask groundMask;
    public LayerMask pendulumMask;


    // 10 seconds before you regain a dash.
    private float timeBetweenDashes = 10f;
    private float timeSinceLastDash;


    private PlayerInput input;
    private Vector2 moveVector;
    private Vector2 lookVector;

    // Number of seconds dash lasts for.
    private float dashLength = 0.2f;

    // Dash time taken so far.
    private float dashCounter = 0f;
    private float dashMultiplier = 10f;

    public SoundManager soundManager;
    private float mouseSensitivity = 2f;
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
    private bool jumping = false;
    private bool readyToJump = true;

    private float jumpCooldown = 0.25f;

    public ParticleSystem cameraParticleSystem;

    private State state = State.Normal;

    public enum State
    {
        Normal,
        Hookshot
    }

    void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        pauseMenu = GameObject.FindObjectOfType<PauseMenu>();
        GrapplingGun grapplingGun = GameObject.FindObjectOfType<GrapplingGun>();

        input = new PlayerInput();
        input.Enable();

        input.Player.Move.performed += context => moveVector = context.ReadValue<Vector2>();
        
        input.Player.Move.canceled += context => moveVector = Vector2.zero;

        input.Player.Look.performed += context => lookVector = context.ReadValue<Vector2>();
        input.Player.Look.canceled += context => lookVector = Vector2.zero;

        input.Player.Jump.performed += context => jumping = true;
        input.Player.Jump.canceled += context => jumping = false;
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
                numTimeWarps--;
                TimeWarp();
            }
        };
        input.Player.RestartLevel.performed += context => gameManager.RestartLevel();

        input.Player.Pause.performed += context => pauseMenu.PressPause();

        input.Player.GrappleShoot.performed += context => grappleShoot = true;
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

        dashMovementSpeed = normalMovementSpeed*dashMultiplier;

        timeSinceLastDash = timeBetweenDashes;

        // Initialize player movement and look vectors to (0, 0).
        lookVector = Vector2.zero;
        cameraRotation = Vector2.zero;

        numDeaths = 0;
        numDashes = 100;
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
                float dashUpMultiplier = 0.4f;
                rigidBody.AddForce(new Vector3(0, jumpForce * dashUpMultiplier, 0), ForceMode.Impulse);

            }
            
            dashCounter -= Time.fixedUnscaledDeltaTime;
            movementSpeed = dashMovementSpeed;
           
        }
        else
        {
            if (dashing) {
                rigidBody.velocity = Vector3.zero;
            }
            dashing = false;
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

    /* private void OnCollisionEnter(Collision collision)
    {
        
        //If the player has entered a collision with a platform, they are grounded.
        
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = true;
            jumping = false;
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
    }*/

    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (groundMask == (groundMask | (1 << layer))) {
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;
                //FLOOR
                if (IsFloor(normal))
                {
                    grounded = true;
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

        if (pendulumMask == (pendulumMask | (1 << layer)))
        {
            for (int i = 0; i < other.contactCount; i++)
            {

                float magnitude = 1000f;
                //Vector3 force = transform.position - collision.transform.position;
                Vector3 force = other.contacts[i].normal;
                force.z = 0;
                force.Normalize();
                this.rigidBody.AddForce(magnitude * force);
                Debug.Log("Here:  " + magnitude * force);

            }
        }

    }
    private void Jump()
    {
        // Added "&& rigidBody" because Jump() was being called when rigidBody was null.


        if (grounded && readyToJump)
        {
            readyToJump = false;
            soundManager.PlayJumpSound();
            rigidBody.AddForce(Vector2.up * jumpForce * 1.5f);
            rigidBody.AddForce(Vector3.up * jumpForce * 0.5f);

            Vector3 vel = rigidBody.velocity;
            if (rigidBody.velocity.y < 0.5f)
                rigidBody.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rigidBody.velocity.y > 0)
                rigidBody.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        else { Debug.Log("Jump failded. Not on ground"); }
        
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

    private void AdjustCamera()
    {
        // Rotate the camera based on mouse movement.

        cameraRotation.x = Mathf.Repeat(cameraRotation.x + lookVector.x * mouseSensitivity * Time.fixedDeltaTime, 360);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookVector.y * mouseSensitivity * Time.fixedDeltaTime, -maxYAngle, maxYAngle);
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);

        // Rotate the player about the Y axis based on the camera's rotation.
        this.transform.eulerAngles = new Vector3(0, cameraRotation.x, 0);
    }

    public Vector2 VelRelativeToLook()
    {
        float lookAngle = transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rigidBody.velocity.x, rigidBody.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rigidBody.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private void Move()
    {
        //Extra gravity
        rigidBody.AddForce(Vector3.down * Time.deltaTime * 10);

        Debug.Log(rigidBody.velocity);
        
        //velocity relative to where player is looking
        Vector2 mag = VelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

      

        // If dash is enabled and your joystick isn't as far out as it could be, you should still go the same dash speed.
        if (dashCounter > 0)
        {
            moveVector.Normalize();
        }

        float maxspeed = this.maxSpeed;
        float x = moveVector.x, y = moveVector.y;

        CounterMovement(x, y, mag);

        if (jumping && readyToJump) Jump();

        if (x > 0 && xMag > maxSpeed  && !dashing) x = 0;
        if (x < 0 && xMag < -maxSpeed  && !dashing) x = 0;
        if (y > 0 && yMag > maxSpeed   && !dashing) y = 0;
        if (y < 0 && yMag < -maxSpeed  && !dashing) y = 0;


        Debug.Log(moveVector.ToString());
 
        float multiplier = 1f, multiplierV =1f;
        if (!grounded)
        {
            Debug.Log("!ground");
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
        rigidBody.AddForce(transform.forward * y * normalMovementSpeed * Time.deltaTime * multiplier * multiplierV ) ;
        rigidBody.AddForce(transform.right * x * normalMovementSpeed * Time.deltaTime * multiplier * multiplierV);


        /* Bob the camera up and down if the player is moving
         * (intentionally, not being pushed) and not grounded.
         */
        bobbing = (moveVector.magnitude > 0 && grounded);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || dashing|| jumping) return;

        //Counter movement
        if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rigidBody.AddForce(normalMovementSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
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
        //Dash();
        
        
    }

    void Update() {
        AdjustCamera();
       
    }
}
