﻿using System.Collections;
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
    private float delayTimer = 3.0f;  // Used to be 1.
    private PlayerControl playerScript;

    void Start()
    {
        playerScript = FindObjectOfType<PlayerControl>();
        readyToShoot = true;
        readyToShoot2 = false;
        StartCoroutine(delay());
    }

    void Update()
    {
        if (targetLocked)
        {
            Vector2 mag = playerScript.VelRelativeToLook();
            Vector3 playerVelocity = new Vector3(mag.x, mag.y, 0);
            TurretMovable.transform.LookAt(target.transform.position + playerVelocity * 1.0f);
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
            // targetLocked = false;
            // target = null;
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

    void OnTriggerExit(Collider other)
    {
        // Stop shooting at the player when they're far away.
        if (other.tag == "Player")
        {
            targetLocked = false;
        }
    }
}