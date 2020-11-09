using System.Collections;
using UnityEngine;

public class LaserControl : MonoBehaviour
{
    public GameObject firePoint;
    private Vector3 startPos; 
    private Vector3 endPos;
    public LineRenderer lr;
    private LineRenderer line;
    private float fireTimer = 0.8f;
    private bool readyToFire;
    public float range;
    private GameManager gameManager;
    private PlayerControl playerControl;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        readyToFire = false;
    }


    void Update()
    {
        // lr.SetPosition(0, firePoint.transform.position);
        // RaycastHit hit;
        if (readyToFire)
        {
            LaserFire();
        }

    }

    void LaserFire()
    {
        if (line == null)
        {
            createLine();
            line.SetPosition(0, firePoint.transform.position);
            startPos = firePoint.transform.position;
        }
        else 
        {
            addColliderToLine();
            line = null;
        }
        readyToFire = false;
        StartCoroutine(FireRate());

    }

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
    }

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
    }


    IEnumerator FireRate()
    {
        yield return new WaitForSeconds(fireTimer);
        readyToFire = true;

    }

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
}
