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

    private float jumpForce = 11f;
    private float doubleJumpForce = 9f;
    private bool ableToDoubleJump = false;
    private bool grounded = true;

    private float movementSpeed;
    private float normalMovementSpeed = 500f;
    private float dashMovementSpeed;

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

        input.Player.Jump.performed += context => Jump();
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
            dashAvailable = false;
            dashCounter = dashLength;
            soundManager.PlayDashSound();
            movementSpeed = dashMovementSpeed;
            cameraParticleSystem.Play();
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

        dashCounter = Mathf.Min(dashCounter, 0f);
        if (dashCounter == 0)
        {
            // After the delay, stop the dash.
            movementSpeed = normalMovementSpeed;
            cameraParticleSystem.Stop();
            dashCooldownCounter = dashCooldownLength;
        }

        if (dashCooldownCounter > 0)
        {
            dashCooldownCounter -= Time.fixedUnscaledDeltaTime;
        }

        dashCooldownCounter = Mathf.Min(dashCooldownCounter, 0f);
        if (dashCooldownCounter == 0)
        {
            dashAvailable = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the player collides with a platform, they are grounded.
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // If the player has exited a collision with a platform, they are no longer grounded.
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = false;

            // Able to double jump after you've left a platform.
            ableToDoubleJump = true;
        }
    }

    private void Jump()
    {
        // TODO:  Added "&& rigidBody" because Jump() was being called when rigidBody was null.  Figure out why this happens!
        if (this.grounded && rigidBody)
        {
            soundManager.PlayJumpSound();
            rigidBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
        else if (!this.grounded && ableToDoubleJump)
        {
            soundManager.PlayDoubleJumpSound();
            // TODO:  Same thing here.
            if (rigidBody)
            {
                rigidBody.AddForce(new Vector3(0, doubleJumpForce, 0), ForceMode.Impulse);
        }
        ableToDoubleJump = false;
        }
    }

    /**
     * Rotate the camera based on mouse/joystick movement.
     */
    private void AdjustCamera()
    {
        cameraRotation.x = Mathf.Repeat(cameraRotation.x + lookVector.x * gameManager.GetSensitivity(), 360);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookVector.y * gameManager.GetSensitivity(), -maxYAngle, maxYAngle);
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);

        // Rotate the player about the Y axis based on the camera's rotation.
        transform.eulerAngles = new Vector3(0, cameraRotation.x, 0);
    }

    /**
     * Move the player based on the controller/keyboard input.
     */
    private void Move()
    {
        // If dash is enabled and your joystick isn't as far out as it could be, you should still go the same dash speed.
        if (dashCounter > 0)
        {
            moveVector.Normalize();
        }

        running = moveVector.magnitude > 0 && grounded;

        // Do running animation if the player is running.
        handsAnimator.SetBool("Running", running);

        // Temporary fix
        float facingAngle = transform.eulerAngles.y * Mathf.PI / 180f;
        float forwardMovement = (moveVector.y * Mathf.Cos(facingAngle) - moveVector.x * Mathf.Sin(facingAngle)) * movementSpeed * Time.fixedUnscaledDeltaTime;
        float horizontalMovement = (moveVector.y * Mathf.Sin(facingAngle) + moveVector.x * Mathf.Cos(facingAngle)) * movementSpeed * Time.fixedUnscaledDeltaTime;
        transform.Translate(new Vector3(horizontalMovement / 25f, 0, forwardMovement / 25f), Space.World);
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

    void FixedUpdate()
    {
        MaintainDash();
        Move();
        AdjustCamera();
    }
}
