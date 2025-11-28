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

    [Header("Recoil Settings")]
    public float recoilAmount = 1.0f;
    public float recoilRecoverySpeed = 2.0f;
    public float maxRecoilAngle = 5.0f;

    private Vector3 currentRecoil = Vector3.zero;
    private Vector3 targetRecoil = Vector3.zero;

    //audioMgmt
    GameObject audioManager;
    void Start()
    {
        currentAmmo = maxAmmo;

        // Auto-find camera if not assigned
        if (fpsCam == null)
        {
            fpsCam = Camera.main;
        }
        // Find Audio Manager
        audioManager = GameObject.Find("AudioManager");
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

        HandleRecoil();
    }

    public void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        currentAmmo--;
        muzzleFlash.Play();
        audioManager.GetComponent<AudioController>().playPlasmaGunSound();
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

    void HandleRecoil()
    {
        // Always apply current recoil to camera
        fpsCam.transform.localRotation = Quaternion.Euler(currentRecoil);

        // Gradually reduce recoil over time
        if (currentRecoil != Vector3.zero)
        {
            currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
        }
    }


    void AddRecoil()
    {
        // Generate random recoil pattern
        float verticalRecoil = Random.Range(0.5f, 1.0f) * recoilAmount;
        float horizontalRecoil = Random.Range(-0.5f, 0.5f) * recoilAmount;

        Vector3 newRecoil = new Vector3(-verticalRecoil, horizontalRecoil, 0);
        currentRecoil += newRecoil;

        // Clamp the maximum recoil
        currentRecoil.x = Mathf.Clamp(currentRecoil.x, -maxRecoilAngle, maxRecoilAngle);
        currentRecoil.y = Mathf.Clamp(currentRecoil.y, -maxRecoilAngle, maxRecoilAngle);
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