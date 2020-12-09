using UnityEngine;

public class TurretPlayerTrigger : MonoBehaviour
{
    private TurretControl turretControl;

    private void Start()
    {
        turretControl = GetComponentInChildren<TurretControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            turretControl.PlayerNearby();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            turretControl.PlayerNoLongerNearby();
        }
    }
}
