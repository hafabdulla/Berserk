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
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect; 

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
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        currentAmmo--;
        muzzleFlash.Play();

        if (gunAnimator != null)
            gunAnimator.SetTrigger("Shoot");

        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
                target.TakeDamage(damage);

            if (impactEffect != null)
            {
                Vector3 pos = hit.point + hit.normal * 0.02f;
                Quaternion rot = Quaternion.LookRotation(hit.normal);
                Instantiate(impactEffect, pos, rot);
            }
        }
        else
        {
            Debug.Log("Raycast hit NOTHING");
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