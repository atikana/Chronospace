using System.Collections;
using System.Collections.Generic;
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
    GameManager gameManager;
    PlayerControl playerControl;
    // Start is called before the first frame update
    void Start()
    {
        readyToFire = false;
    }

    // Update is called once per frame
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
        line.SetVertexCount(2);
        line.SetWidth(5.0f, 5.0f);
        line.SetColors(Color.black, Color.black);
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
            playerControl.AddDeath();
            gameManager.RestartLevel();
        }
        else
        {
            // Destroy(this.gameObject);
        }
    }
}
