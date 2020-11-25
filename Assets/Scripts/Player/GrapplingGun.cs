using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private const float NORMAL_FOV = 90f;
    private const float HOOKSHOT_FOV = 120f;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private Vector3 grappleDir;
    private GrappleHolder holder;
    private CameraFov cameraFov;
    public LayerMask whatIsGrappleable;
    public GameObject aimPoint;

    // The grapple rope shoots out of the player's left wrist.
    public Transform playerWristTransform;

    private SoundManager soundManager;
    private PlayerControl playerControl;
    private ParticleSystem cameraParticleSystem;

    private bool pulling = false;
    public float pullingMomentumMultiplier = 0.5f;
    public float maxPullingSpeed = 50f;

    // Use a counter to shoot the rope after the grappling animation.
    private float ropeShootCounter = 0f;
    private const float ropeShootLength = 0.3f;

    public Transform cameraTransform;
    public float sphereRadius;
    public Transform sphere;
    private Transform playerTransform;
    private Rigidbody playerBody;
    private float maxDistance = 100f;

    private SpringJoint joint;
    private RaycastHit grappleHit;

    // The player's current state - either normal or grappling.
    private GrapplingState grapplingState = GrapplingState.Normal;

    // For animating the hands when the player grapples.
    private Animator handsAnimator;

    private GameObject lastGrapple;
    private bool showRope = false;
    private Color original = new Color(255, 255, 255);
    private Color neonGreen = new Color(28, 191, 42);

    bool autoAim = false;

    public Crosshair crosshair;

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
        handsAnimator = transform.parent.GetComponentInChildren<Animator>();
        soundManager = FindObjectOfType<SoundManager>();
        playerControl = transform.parent.GetComponent<PlayerControl>();
        cameraParticleSystem = transform.parent.GetComponentInChildren<ParticleSystem>();
        playerTransform = transform.parent;

        playerBody = transform.parent.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //GrappleAim();
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

    public bool isGrapping()
    {
        return grapplingState == GrapplingState.Grappling;
    }

    void LateUpdate()
    {
        WaitToGrapple();

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

    void GrappleAim()
    {
        if (grapplingState == GrapplingState.Normal)
        {
            GameObject close;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out grappleHit, maxDistance, whatIsGrappleable))
            {

                autoAim = false;
                aimPoint.transform.position = grappleHit.point;
                aimPoint.SetActive(true);
                grapplePoint = grappleHit.point;
            }
            else if (playerControl.GetGrapplingAutoAimStatus() && (close = FindGrapplePoint()) != null)
            {
                // should i make it actually shoot to the player
              
                aimPoint.transform.position = close.transform.position;
                aimPoint.SetActive(true);
            }
            else
            {
                aimPoint.SetActive(false);
            }
        }
       
    }


    void StartGrapple()
    {
        if (grapplingState == GrapplingState.Normal)
        {
            GameObject close;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out grappleHit, maxDistance, whatIsGrappleable))
            {
                showRope = true;
                lr.positionCount = 2;
                //autoAim = false;
                StartGrappleHelper(grappleHit.collider.gameObject);
                grapplePoint = grappleHit.point;
            }
            else if (playerControl.GetGrapplingAutoAimStatus() && (close = FindGrapplePoint()) != null)
            {
                // should i make it actually shoot to the player
                showRope = true;
                lr.positionCount = 2;
                //autoAim = true;
                StartGrappleHelper(close.gameObject);
                grapplePoint = close.GetComponent<Renderer>().bounds.center;
            }
        }
    }


  /*  void StartGrapple()
    {
        if (grapplingState == GrapplingState.Normal)
        {
            GameObject close;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out grappleHit, maxDistance, whatIsGrappleable))
            {
                showRope = true;
                lr.positionCount = 2;
                autoAim = false;
                aimPoint.transform.position = grappleHit.point;
                aimPoint.SetActive(true);
                StartGrappleHelper(grappleHit.collider.gameObject);
                grapplePoint = grappleHit.point;
            }
            else if (playerControl.GetGrapplingAutoAimStatus() && (close = FindGrapplePoint()) != null)
            {
                // should i make it actually shoot to the player
                showRope = true;
                lr.positionCount = 2;
                autoAim = true;
                aimPoint.transform.position = close.transform.position;
                aimPoint.SetActive(true);
                StartGrappleHelper(close.gameObject);
                grapplePoint = lastGrapple.transform.position;
            }
        }
        else
        {
            aimPoint.SetActive(false);
        }
    } */

    void StartGrappleHelper(GameObject g)
    {
        grapplingState = GrapplingState.Grappling;

        // Wait for the counter to hit zero before the grapple takes place.
        ropeShootCounter = ropeShootLength;

        // Only animate the hands if you shoot the grappling gun.
        handsAnimator.ResetTrigger("StopGrappling");
        handsAnimator.SetTrigger("Grappling");
        soundManager.PlayGrapplingSound();
        lastGrapple = g;
        g.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", neonGreen * 0.0150f);

        crosshair.ChangeCrossHairColor();
    }

    private void WaitToGrapple()
    {
        // Start grappling once the counter gets to zero.
        if (ropeShootCounter > 0)
        {
            ropeShootCounter -= Time.unscaledDeltaTime;
            if (ropeShootCounter <= 0)
            {
                ropeShootCounter = 0f;
                ShootGrapple();
            }
        }
    }

    private void ShootGrapple()
    {
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
    }

    /**
     * Set up the grappling rope.
     */
    void DrawRope()
    {
        if (!showRope)
        {
            return;
        }

        lr.SetPosition(0, playerWristTransform.position);
        if (ropeShootCounter > 0)
        {
            Vector3 ropeEndpoint = (ropeShootLength - ropeShootCounter) * grapplePoint + ropeShootCounter * playerWristTransform.position;
            ropeEndpoint /= ropeShootLength;
            lr.SetPosition(1, ropeEndpoint);
        }
        else
        {
            lr.SetPosition(1, grapplePoint);
        }
    }

    public void StopGrapple()
    {
        showRope = false;
        grapplingState = GrapplingState.Normal;
        lr.positionCount = 0;
        Destroy(joint);
        handsAnimator.ResetTrigger("Grappling");
        handsAnimator.SetTrigger("StopGrappling");
        crosshair.RevertCrosshairColor();
        if (lastGrapple != null)
        {
            lastGrapple.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", original * 0.0150f);
        }
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

    public void ResetRope()
    {
        Debug.Log("Reset Rope");

        if (!pulling)
        {
            return;
        }

        if (playerBody.velocity.magnitude > maxPullingSpeed)
        {
            playerBody.velocity = Vector3.ClampMagnitude(playerBody.velocity, maxPullingSpeed);
        }
        playerBody.velocity = playerBody.velocity * pullingMomentumMultiplier;

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

    GameObject FindGrapplePoint()
    {
        Collider[] hits = Physics.OverlapSphere(sphere.position, sphereRadius);
        float temp = float.MaxValue;

        GameObject closeGrapplable = null;

        GameObject secondClose = null;


        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("grapple"))
            {
                RaycastHit hitObject;
                if (Physics.Raycast(playerTransform.position, hit.transform.position, out hitObject, 100))
                {
                    Debug.DrawRay(playerTransform.position, hit.transform.position);
                    // if it not small platform and not hitting itself
                    if (!(hit.name.CompareTo("pCube3") == 0 && hitObject.collider.transform.parent != hit.transform.parent))
                    {
                        continue;
                    }
                 
                
                   
                }
                
                // so u dont hit the same target, unless u have to or u r already on the ground.
                if (lastGrapple != null && lastGrapple.name.CompareTo(hit.name) == 0 && lastGrapple.name.CompareTo("pCube3") == 0 && lastGrapple.transform.parent == hit.transform.parent && !playerControl.GetGroundStatus())
                {
                    secondClose = hit.gameObject;
                    continue;
                }


                 
                // find the smallest grappling point that is not the one detecting
                float distance = (cameraTransform.position - hit.transform.position).sqrMagnitude;

                if (distance< temp)
                {
                    temp = distance;
                    closeGrapplable = hit.gameObject;

                   
                }
            }
        }

        if (closeGrapplable != null)
        {
            Debug.Log("Next target" + closeGrapplable.name + "of" + closeGrapplable.transform.parent.name);
        }
        else if (secondClose != null)
        {
            closeGrapplable = secondClose;
            //Debug.Log("Doesn't have anything to grab" + secondClose.name);
        }



        return closeGrapplable;
        

    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
      // Gizmos.color = Color.yellow;
      // Gizmos.DrawSphere(sphere.position, sphereRadius);
    }
}