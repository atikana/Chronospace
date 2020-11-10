using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Player's optimal rotation when starting at this checkpoint.
    public float playerRotation;

    public Vector3 GetCheckPointPosition()
    {
        return transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.parent.GetComponent<CheckPointManager>().AddCheckPoint(this);
        }
    }
}
