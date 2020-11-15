using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    // Player's rotation when starting at this checkpoint.
    private float playerRotation = 0f;
    private PlayerControl playerControl;

    private void Start()
    {
        playerControl = FindObjectOfType<PlayerControl>();
    }

    public Vector3 GetCheckPointPosition()
    {
        return transform.position;
    }

    public float GetPlayerRotation()
    {
        return playerRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.parent.GetComponent<CheckPointManager>().AddCheckPoint(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            /* Change the player's rotation while they're inside the checkpoint, so that if they
             * die before leaving the checkpoint they'll respawn at a reasonable rotation.
             */
            playerRotation = playerControl.GetCameraRotation().x;
        }
    }
}
