using System.Collections;
using UnityEngine;

public class LaserControl : MonoBehaviour
{
    public GameObject firePoint;
    public LineRenderer lr;
    private float fireTimer = 3.0f;
    private float enhanceTimer = 0.8f;
    private bool readyToFire;
    private bool enhanceFire;
    private SoundManager soundManager;
    private GameManager gameManager;

    private Transform playerTransform;

    private bool laserBeamSoundOn = false;

    // If player is within this distance of a laser, it will play a sound.
    private const float laserSoundThreshold = 50f;

    private void Awake()
    {
        playerTransform = FindObjectOfType<PlayerControl>().transform;
        soundManager = FindObjectOfType<SoundManager>();
        gameManager = FindObjectOfType<GameManager>();
        readyToFire = true;
        enhanceFire = false;
    }


    void FixedUpdate()
    {
        // lr.SetPosition(0, firePoint.transform.position);
        // RaycastHit hit;
        fireTimer = 3f;
        enhanceTimer = 0.8f;

        if (readyToFire)
        {
            // Debug.Log("Laser created");
            LaserFire();
        }
        if (enhanceFire)
        {
            if (Random.Range(0.0f, 1.0f) >= 0.5f)
            {
                Transform line_ = Instantiate(lr.transform, firePoint.transform.position, Quaternion.identity);
                line_.transform.rotation = firePoint.transform.rotation;
            }
        }

        // Play the laser sound, if necessary.
        if (Vector3.Distance(playerTransform.position, transform.position) < laserSoundThreshold)
        {
            if (!laserBeamSoundOn)
            {
                soundManager.StartLaserSound(transform.position);
            }
            laserBeamSoundOn = true;
        }
        else
        {
            if (laserBeamSoundOn)
            {
                soundManager.StopLaserSound(transform.position);
            }
            laserBeamSoundOn = false;
        }
    }

    void LaserFire()
    {   
        Transform line = Instantiate(lr.transform, firePoint.transform.position, Quaternion.identity);
        line.transform.rotation = firePoint.transform.rotation;
        readyToFire = false;
        StartCoroutine(FireRate());
        enhanceFire = true;
        StartCoroutine(EnhanceFireRate());
    }

    /*
    private void createLine()
    {
        line = new GameObject("Line").AddComponent<LineRenderer>();
        //line.material = new Material(Shader.Find("Diffuse"));
        line.positionCount = 2;
        line.startWidth = 5f;
        line.endWidth = 5f;
        line.startColor = Color.gray;
        line.endColor = Color.gray;
        line.useWorldSpace = true;
    }*/

    /*
    private void addColliderToLine()
    {
        BoxCollider col = new GameObject("Collider").AddComponent<BoxCollider>();
        col.transform.parent = line.transform; // Collider is added as child object of line
        float lineLength = Vector3.Distance(startPos, endPos); // length of line
        col.size = new Vector3(lineLength, 0.1f, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos + endPos) / 2;
        col.transform.position = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        col.transform.Rotate(0, 0, angle);
    }*/


    IEnumerator FireRate()
    {
        int i = 0;
        while (i < 30)
        {
            yield return new WaitForSeconds(fireTimer / 30.0f / gameManager.GetTimeWarpMultiplier());
            i++;
        }
        readyToFire = true;
    }

    IEnumerator EnhanceFireRate()
    {
        int i = 0;
        while (i < 10)
        {
            yield return new WaitForSeconds(enhanceTimer / 10.0f / gameManager.GetTimeWarpMultiplier());
            i++;
        }
        enhanceFire = false;

    }


    /*
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameManager.KillPlayer();
        }
        else
        {
            // Destroy(this.gameObject);
        }
    }
    */
}
