using System.Collections;
using UnityEngine;

public class TurretControl : MonoBehaviour
{

    private GameObject target;
    private bool targetLocked;
    public GameObject TurretMovable;
    public GameObject bullet;
    public GameObject muzzle1;
    public GameObject muzzle2;
    private float fireTimer = 0.8f;  // Used to be 2.
    private bool readyToShoot;
    private bool readyToShoot2;
    private float delayTimer = 0.4f;  // Used to be 1.

    // Start is called before the first frame update
    void Start()
    {
        readyToShoot = true;
        readyToShoot2 = false;
        StartCoroutine(delay());
    }

    // Update is called once per frame
    void Update()
    {
        if (targetLocked)
        {
            TurretMovable.transform.LookAt(target.transform);
            // Angle adjustments
            TurretMovable.transform.Rotate(8, 0, 0);

            if (readyToShoot)
            {
                Shoot();
            }
            if (readyToShoot2)
            {
                Shoot2();
            }
        }
    }

    void Shoot()
    {
        Transform _bullet = Instantiate(bullet.transform, muzzle1.transform.position, Quaternion.identity);
        _bullet.transform.rotation = TurretMovable.transform.rotation;
        readyToShoot = false;
        StartCoroutine(FireRate());

    }

    void Shoot2()
    {
        Transform _bullet = Instantiate(bullet.transform, muzzle2.transform.position, Quaternion.identity);
        _bullet.transform.rotation = TurretMovable.transform.rotation;
        readyToShoot2 = false;
        StartCoroutine(FireRate2());

    }

    IEnumerator FireRate()
    {
        yield return new WaitForSeconds(fireTimer);
        readyToShoot = true;
    }

    IEnumerator FireRate2()
    {
        yield return new WaitForSeconds(fireTimer);
        readyToShoot2 = true;
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(delayTimer);
        readyToShoot2 = true;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.gameObject;
            targetLocked = true;
        }
    }
}
