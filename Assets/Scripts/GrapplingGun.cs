using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private Vector3 grappleDir;
    public LayerMask whatIsGrappleable;

    public PlayerControl playerControl; 

    bool pulling = false;

    public Transform gunTip, camera, player;
    public Rigidbody playerBody;
    public float maxDistance = 100f;

    private SpringJoint joint;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (pulling)
            {
                ResetRope();
            }
            StopGrapple();
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (joint)
            {
                PullRope();
            }
        }
        else if (Input.GetMouseButtonUp(1))
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
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            playerControl.ActivateHookShotState();
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance the grapple will try to keep from the point
            joint.maxDistance = distanceFromPoint * 0.5f;
            joint.minDistance = distanceFromPoint * 0.25f;

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
        playerControl.DisableHookShotState();
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


    }

    void ResetRope()
    {

        Debug.Log("Reset Rope");

        playerBody.velocity = Vector3.zero;

        pulling = false;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

    }


}