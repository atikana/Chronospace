using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Player's optimal rotation when starting at this checkpoint.
    public float playerRotation;

    Transform front;
    Transform back;
    Color original = new Color(255, 255, 255);
    Color neonGreen = new Color(7, 299, 3);



    private void Awake()
    {
        front = transform.GetChild(0);
        back = transform.GetChild(1);

        if (name.CompareTo("1") == 0)
        {
            ChangeColor(neonGreen);
        }
    }

    public Vector3 GetCheckPointPosition()
    {
        return transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.parent.GetComponent<CheckPointManager>().AddCheckPoint(this);
            ChangeColor(neonGreen);
        }
    }

    void ChangeColor(Color c)
    {

        front.GetComponent<Renderer>().material.SetColor("_EmissionColor", c * 0.005f);
        back.GetComponent<Renderer>().material.SetColor("_EmissionColor", c * 0.005f);
    }

    public void RevertColor()
    {
        ChangeColor(original);
    }

    
}
