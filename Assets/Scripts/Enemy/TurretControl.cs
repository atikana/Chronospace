using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TurretControl : MonoBehaviour
{
    private bool targetLocked;
    public GameObject bullet;
    public GameObject muzzle1;
    public GameObject muzzle2;
    private float fireTimer = 0.8f;
    private bool readyToShoot;
    private PlayerControl playerScript;
    private SoundManager soundManager;
    private bool turretEnabled = true;
    public Animator turretAnimator;
    private Quaternion startPosition;
    private float aimAheadOfPlayerMultiplier = 8f;

    // Offset when looking at the player to make it seem like the bullet is hitting them higher.
    private Vector3 playerPositionOffset = new Vector3(0f, 5f, 0f);
    private Quaternion bulletRotation;

    private void Awake()
    {
        playerScript = FindObjectOfType<PlayerControl>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Start()
    {
        readyToShoot = true;
        turretAnimator.ResetTrigger("StartShooting");
        turretAnimator.ResetTrigger("StopShooting");
        startPosition = transform.rotation;
    }

    void Update()
    {
        if (targetLocked)
        {
            Vector3 playerNextPosition;
            if (playerScript.GetVelocity().magnitude > 0)
            {
                float playerTurretDistance = Vector3.Distance(transform.position, playerScript.transform.position);
                playerNextPosition = playerScript.transform.position + playerScript.GetVelocity() * aimAheadOfPlayerMultiplier * (1.0f / playerTurretDistance);
            }
            else
            {
                playerNextPosition = playerScript.transform.position;
            }

            transform.LookAt(playerNextPosition - playerPositionOffset);
            bulletRotation = transform.rotation;
            transform.LookAt(new Vector3(playerNextPosition.x, transform.position.y, playerNextPosition.z));

            if (readyToShoot)
            {
                Shoot();
            }
        }
    }

    private void PlayBulletSound(Vector3 muzzleLocation)
    {
        // Only play bullet sound if the player is near the turret.
        soundManager.PlayBulletSound(muzzleLocation);
    }

    void Shoot()
    {
        turretAnimator.SetTrigger("StartShooting");
        turretAnimator.ResetTrigger("StopShooting");

        // Shoot the first bullet.
        Transform _bullet = Instantiate(bullet.transform, muzzle1.transform.position, Quaternion.identity);
        _bullet.transform.rotation = bulletRotation;
        PlayBulletSound(muzzle1.transform.position);

        // Shoot the second bullet, if we are using a double turret.
        if (muzzle2 != null)
        {
            Transform _bullet2 = Instantiate(bullet.transform, muzzle2.transform.position, Quaternion.identity);
            _bullet2.transform.rotation = bulletRotation;
        }

        readyToShoot = false;
        StartCoroutine(FireRate());
    }

    IEnumerator FireRate()
    {
        yield return new WaitForSeconds(fireTimer);
        turretAnimator.ResetTrigger("StartShooting");
        turretAnimator.SetTrigger("StopShooting");
        readyToShoot = true;
    }

    public void EnableTurret()
    {
        turretEnabled = true;
    }

    public void DisableTurret()
    {
        turretEnabled = false;
        targetLocked = false;
        transform.rotation = startPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (turretEnabled && other.tag == "Player")
        {
            targetLocked = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Stop shooting at the player when they're far away.
        if (other.tag == "Player")
        {
            targetLocked = false;
            transform.rotation = startPosition;
        }
    }

    public void ResetTurret()
    {
        targetLocked = false;
        transform.rotation = startPosition;
    }
}
