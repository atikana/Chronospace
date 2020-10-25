using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private const float NORMAL_FOV = 60f;
    private const float HOOKSHOT_FOV = 100f;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private Vector3 grappleDir;
    public CameraFov cameraFov;
    public LayerMask whatIsGrappleable;

    public PlayerControl playerControl;
    public ParticleSystem cameraParticleSystem;

    bool pulling = false;

    public Transform gunTip, mainCamera, player;
    public Rigidbody playerBody;
    public float maxDistance = 100f;

    private SpringJoint joint;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
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
            grappleDir = (grapplePoint - gunTip.position).normalized;
            float hookshotSpeed = Vector3.Distance(gunTip.position, grapplePoint);
            //Change the pull speed
            float hookshotSpeedMultiplier = 250f;
            playerBody.velocity = grappleDir * hookshotSpeed * hookshotSpeedMultiplier * Time.fixedDeltaTime;
        }

        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            //playerControl.ActivateHookShotState();
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

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

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);

    }

    void StopGrapple()
    {
        //playerControl.DisableHookShotState();
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

        grappleDir = (grapplePoint - gunTip.position).normalized;
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

        if (joint) {
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }

    }


}