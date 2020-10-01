using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public Rigidbody rigidBody;
    private float jumpForce = 5f;
    private float horizontalMovementSpeed = 10f;
    private float forwardMovementSpeed = 10f;
    private bool grounded = true;
    private int dashCounter = 0;
    public SoundManager soundManager;
    private float mouseSensitivity = 3f;
    public Transform cameraTransform;
    private Vector2 cameraRotation;
    private float maxYAngle = 90f;
    private int timeWarpCounter = 0;
    public Animator pendulumAnimator1;
    public Animator pendulumAnimator2;
    public Animator pendulumAnimator3;
    public Animator pendulumAnimator4;
    public Animator droneAnimator1;
    public Animator droneAnimator2;
    public Animator droneAnimator3;
    public Animator droneAnimator4;
    private float deletethis;

    void Start()
    {
        // Don't show user's cursor.
        Cursor.visible = false;

        // Camera starts out facing forward.
        cameraRotation = new Vector2(0, 0);
    }

    private void TimeWarp()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            soundManager.PlayTimeWarpSound();
            GameManager.SetTimeWarp();
            this.deletethis = droneAnimator4.speed;
            Debug.Log(deletethis);
            this.timeWarpCounter = 360;
            pendulumAnimator1.speed /= 2;
            pendulumAnimator2.speed /= 2;
            pendulumAnimator3.speed /= 2;
            pendulumAnimator4.speed /= 2;
            droneAnimator1.speed /= 2;
            droneAnimator2.speed /= 2;
            droneAnimator3.speed /= 2;
            droneAnimator4.speed /= 2;
        }
        if (timeWarpCounter > 1)
        {
            timeWarpCounter--;
        }
        else if (timeWarpCounter == 1)
        {
            deletethis = 1f;
            pendulumAnimator1.speed = deletethis;
            pendulumAnimator2.speed = deletethis;
            pendulumAnimator3.speed = deletethis;
            pendulumAnimator4.speed = deletethis;
            droneAnimator1.speed = deletethis;
            droneAnimator2.speed = deletethis;
            droneAnimator3.speed = deletethis;
            droneAnimator4.speed = deletethis;
        }
        else
        {
            GameManager.RemoveTimeWarp();
        }
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            soundManager.PlayDashSound();
            dashCounter = 60;
        }
        if (dashCounter > 0)
        {
            dashCounter--;
            forwardMovementSpeed = 30f;
        }
        else
        {
            forwardMovementSpeed = 10f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*
         * Loop through platform GameObjects.  If the player is entering a collision with one, they are grounded.
         */
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = true;
        }
        else if (collision.gameObject.tag.Equals("Pendulum"))
        {
            /*PendulumControl pendulumControl2 = (PendulumControl)collision.gameObject.transform.parent.GetComponent(typeof(PendulumControl));
            Debug.Log(pendulumControl2.getPlayerForceVector());
            rigidBody.AddForce(-500 * pendulumControl2.getPlayerForceVector());*/

            //foreach (PendulumControl pendulumControl in FindObjectsOfType<PendulumControl>())
            //{
            //    if (GameObject.ReferenceEquals(pendulumControl.gameObject, collision.gameObject))
            //    {
            //        Debug.Log(pendulumControl.getPlayerForceVector());
            //        rigidBody.AddForce(pendulumControl.getPlayerForceVector());
            //    }
            //}

            //Debug.Log(((PendulumControl)collision.gameObject.transform.parent.GetComponent(typeof(PendulumControl))) == null);
            //Debug.Log(collision.impulse);
            //if (collision.impulse.x < 0)
            //{
            //    this.rigidBody.AddForce(new Vector3(-500, 0, 0));
            //}
            //else
            //{
            //    this.rigidBody.AddForce(new Vector3(500, 0, 0));
            //}
        }
        //foreach (GameObject pendulum in GameObject.FindGameObjectsWithTag("Pendulum"))
        //{
        //    if (GameObject.ReferenceEquals(pendulum, collision.gameObject))
        //    {
        //        //Debug.Log(pendulum.GetComponent<Rigidbody>().angularVelocity);
        //        Debug.Log(((PendulumControl)pendulum.transform.parent.GetComponent(typeof(PendulumControl))) == null);
        //        Vector3 velocity = ((PendulumControl)pendulum.transform.parent.GetComponent(typeof(PendulumControl))).getPlayerForceVector();
        //        Debug.Log(velocity);
        //    }
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        /*
         * Loop through platform GameObjects.  If the player has exited a collision with one, they are no longer grounded.
         */
        if (collision.gameObject.tag.Equals("Platform"))
        {
            this.grounded = false;
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && this.grounded)
        {
            soundManager.PlayJumpSound();
            this.rigidBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }

    private void AdjustCamera()
    {
        // Rotate the camera based on mouse movement.
        cameraRotation.x = Mathf.Repeat(cameraRotation.x + Input.GetAxis("Mouse X") * mouseSensitivity, 360);
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - Input.GetAxis("Mouse Y") * mouseSensitivity, -maxYAngle, maxYAngle);
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0);

        // Rotate the player about the Y axis based on the camera's rotation.
        this.transform.eulerAngles = new Vector3(0, cameraRotation.x, 0);
    }

    private void Move()
    {
        float facingAngle = transform.eulerAngles.y * Mathf.PI / 180f;

        float forwardSpeed = Input.GetAxis("Vertical") * forwardMovementSpeed * Mathf.Cos(facingAngle) - Input.GetAxis("Horizontal") * forwardMovementSpeed * Mathf.Sin(facingAngle);
        float horizontalSpeed = Input.GetAxis("Vertical") * horizontalMovementSpeed * Mathf.Sin(facingAngle) + Input.GetAxis("Horizontal") * horizontalMovementSpeed * Mathf.Cos(facingAngle);
        rigidBody.velocity = new Vector3(horizontalSpeed,
                                         rigidBody.velocity.y,
                                         forwardSpeed);
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        TimeWarp();
        Dash();
        AdjustCamera();
    }
}
