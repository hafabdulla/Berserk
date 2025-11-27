using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.1f; // Time between shots
    public int maxAmmo = 30;
    public int currentAmmo;

    [Header("References")]
    public Camera fpsCam; // The main camera
    public ParticleSystem muzzleFlash; // Optional: visual effect
    public GameObject impactEffect; // Optional: bullet hole effect

    [Header("Animation")]
    public Animator gunAnimator; // We'll set this up in Part 2

    private float nextTimeToFire = 0f;

    void Start()
    {
        currentAmmo = maxAmmo;

        // Auto-find camera if not assigned
        if (fpsCam == null)
        {
            fpsCam = Camera.main;
        }
    }

    void Update()
    {
        // Check if player is shooting
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

        // Reload (optional - for later)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void Shoot()
    {
        // Check ammo
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        currentAmmo--;

        Debug.Log("Shooting! Ammo left: " + currentAmmo);

        // Play muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Play shooting animation
        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Shoot");
        }

        // Raycast from center of screen
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // Check if we hit an enemy or target
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            // Spawn impact effect (bullet hole, sparks, etc.)
            if (impactEffect != null)
            {
                GameObject impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 2f); // Remove after 2 seconds
            }
        }
    }

    void Reload()
    {
        Debug.Log("Reloading...");
        currentAmmo = maxAmmo;

        // Play reload animation
        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Reload");
        }
    }
}