using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private const float NORMAL_FOV = 60f;
    private const float HOOKSHOT_FOV = 100f;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private Vector3 grappleDir;
    private CameraFov cameraFov;
    public LayerMask whatIsGrappleable;

    private SoundManager soundManager;
    private PlayerControl playerControl;
    private ParticleSystem cameraParticleSystem;

    private bool pulling = false;

    public Transform cameraTransform;
    private Transform playerTransform;
    private Rigidbody playerBody;
    private float maxDistance = 100f;

    private SpringJoint joint;

    // The player's current state - either normal or grappling.
    private GrapplingState grapplingState = GrapplingState.Normal;

    // For animating the hands when the player grapples.
    private Animator handsAnimator;

    /**
     * Enumeration of grappling states.
     */
    private enum GrapplingState
    {
        Normal,
        Grappling
    }

    void Awake()
    {
        cameraFov = transform.parent.GetComponentInChildren<CameraFov>();
        lr = GetComponent<LineRenderer>();
        handsAnimator = FindObjectOfType<Animator>();
        soundManager = FindObjectOfType<SoundManager>();
        playerControl = transform.parent.GetComponent<PlayerControl>();
        cameraParticleSystem = transform.parent.GetComponentInChildren<ParticleSystem>();
        playerTransform = transform.parent;
        playerBody = transform.parent.GetComponent<Rigidbody>();
    }

    void Update()
    {
        /*if (Input.GetAxis("Fire1") == 1)
        {
            if (!joint) { StartGrapple(); }

        }
        else if (Input.GetAxis("Fire1") == 0)
        {
            if (pulling)
            {
                ResetRope();
            }

            if (joint)
            {
                StopGrapple();
            }
        }

        if (Input.GetAxis("Fire2") == 1)
        {
            if (joint)
            {
                PullRope();

            }
        }
        else if (Input.GetAxis("Fire2") == 0)
        {
            if (joint && pulling)
            {
                ResetRope();

            }
        }*/

        if (playerControl.GetGrappleShoot())
        {
            if (!joint)
            {
                StartGrapple();
            }

        }
        else
        {
            if (pulling)
            {
                ResetRope();
            }

            if (joint)
            {
                StopGrapple();
            }
        }

        if (playerControl.GetGrappleToggle())
        {
            if (joint)
            {
                PullRope();
            }
        }
        else
        {
            if (joint && pulling)
            {
                ResetRope();
            }
        }

    }

    void LateUpdate()
    {

        if (pulling)
        {
            grappleDir = (grapplePoint - transform.position).normalized;
            float hookshotSpeed = Vector3.Distance(transform.position, grapplePoint);
            //Change the pull speed
            float hookshotSpeedMultiplier = 250f;
            playerBody.velocity = grappleDir * hookshotSpeed * hookshotSpeedMultiplier * Time.fixedDeltaTime;
        }

        DrawRope();
    }

    public bool IsGrappling()
    {
        return grapplingState == GrapplingState.Grappling;
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxDistance, whatIsGrappleable))
        {
            // Only animate the hands if you shoot the grappling gun.
            handsAnimator.SetTrigger("Grappling");
            soundManager.PlayGrapplingSound();
            grapplingState = GrapplingState.Grappling;

            grapplePoint = hit.point;
            joint = playerTransform.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(playerTransform.position, grapplePoint);

            //The distance the grapple will try to keep from the point
            joint.maxDistance = distanceFromPoint * 0.5f;
            joint.minDistance = 0f;

            //Change these value to fit the gameplay
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    void DrawRope()
    {
        if (!joint) return;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, grapplePoint);

    }

    void StopGrapple()
    {
        grapplingState = GrapplingState.Normal;
        lr.positionCount = 0;
        Destroy(joint);
    }

    void PullRope()
    {
        Debug.Log("Pull Rope");

        joint.spring = 0f;
        //joint.damper = 15f;
        //joint.maxDistance = 0f;
        //joint.minDistance = 0f;

        grappleDir = (grapplePoint - transform.position).normalized;
        //joint.minDistance = 0f;
        //joint.maxDistance = 0f;

        pulling = true;

        cameraFov.SetCameraFov(HOOKSHOT_FOV);
        cameraParticleSystem.Play();


    }

    void ResetRope()
    {
        Debug.Log("Reset Rope");

        playerBody.velocity = Vector3.zero;

        pulling = false;

        cameraFov.SetCameraFov(NORMAL_FOV);
        cameraParticleSystem.Stop();

        if (joint)
        {
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }

    }


}
