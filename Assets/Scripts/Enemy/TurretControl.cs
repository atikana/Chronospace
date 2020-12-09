using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

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
    private PlayerControl playerScript;
    private SoundManager soundManager;
    private bool turretEnabled = true;
    public Animator turretAnimator;
    private Quaternion startPosition;

    // If player is within this distance of a turret, bullets will play a sound.
    private const float bulletSoundThreshold = 50f;

    private void Awake()
    {
        playerScript = FindObjectOfType<PlayerControl>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Start()
    {
        readyToShoot = true;
        readyToShoot2 = false;
        turretAnimator.ResetTrigger("StartShooting");
        turretAnimator.ResetTrigger("StopShooting");
        StartCoroutine(delay());
        startPosition = TurretMovable.transform.rotation;
    }

    void Update()
    {
        if (targetLocked)
        {
            Vector2 mag = playerScript.VelRelativeToLook();
            // Vector3 playerVelocity = new Vector3(mag.x / (float)Math.Sqrt(mag.x * mag.x + mag.y * mag.y), mag.y / (float)(Math.Sqrt(mag.x * mag.x + mag.y * mag.y)), 0);
            Vector3 playerVelocity = new Vector3(mag.x , mag.y , 0);
            Vector3 relativePos = TurretMovable.transform.position - target.transform.position;
            Quaternion rotation = Quaternion.LookRotation(target.transform.position, new Vector3(0, 1, 0));
            TurretMovable.transform.localRotation = rotation;
            // TurretMovable.transform.LookAt(target.transform.position + playerVelocity * 1.0f);
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

    private void PlayBulletSound(Vector3 muzzleLocation)
    {
        // Only play bullet sound if the player is near the turret.
        if (Vector3.Distance(playerScript.transform.position, muzzleLocation) < bulletSoundThreshold)
        {
            soundManager.PlayBulletSound(muzzleLocation);
        }
    }

    void Shoot()
    {
        turretAnimator.SetTrigger("StartShooting");
        turretAnimator.ResetTrigger("StopShooting");
        Transform _bullet = Instantiate(bullet.transform, muzzle1.transform.position, Quaternion.identity);
        _bullet.transform.rotation = TurretMovable.transform.rotation;
        readyToShoot = false;
        StartCoroutine(FireRate());
        PlayBulletSound(muzzle1.transform.position);
    }

    void Shoot2()
    {
        Transform _bullet = Instantiate(bullet.transform, muzzle2.transform.position, Quaternion.identity);
        _bullet.transform.rotation = TurretMovable.transform.rotation;
        readyToShoot2 = false;
        StartCoroutine(FireRate2());
        PlayBulletSound(muzzle2.transform.position);
    }

    IEnumerator FireRate()
    {
        yield return new WaitForSeconds(fireTimer);
        turretAnimator.ResetTrigger("StartShooting");
        turretAnimator.SetTrigger("StopShooting");
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

    public void EnableTurret()
    {
        turretEnabled = true;
    }

    public void DisableTurret()
    {
        turretEnabled = false;
        targetLocked = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (turretEnabled && other.tag == "Player")
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
            if (TurretMovable)
            {
                TurretMovable.transform.rotation = startPosition;
            }
        }
    }

    public void ResetTurret()
    {
        targetLocked = false;
        target = null;
        if (TurretMovable)
        {
            TurretMovable.transform.rotation = startPosition;
        }
    }
}
