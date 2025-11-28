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
    public Animator gunAnimator;

    [Header("Movement Detection")]
    public CharacterController playerController; // Reference to player's character controller

    private float nextTimeToFire = 0f;

    void Start()
    {
        currentAmmo = maxAmmo;

        // Auto-find camera if not assigned
        if (fpsCam == null)
        {
            fpsCam = Camera.main;
        }

        // Auto-find character controller if not assigned
        if (playerController == null)
        {
            playerController = GetComponentInParent<CharacterController>();
        }
    }

    void Update()
    {
        // Handle movement animations
        HandleMovementAnimations();

        // Check if player is shooting
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

        // Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void HandleMovementAnimations()
    {
        if (gunAnimator == null) return;

        // Check if player is moving
        bool isMoving = IsPlayerMoving();
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        // Set animator parameters
        gunAnimator.SetBool("IsWalking", isMoving && !isRunning);
        gunAnimator.SetBool("IsRunning", isRunning);

        // Optional: Set float for blend trees if your animation uses them
        float moveSpeed = 0f;
        if (isRunning)
            moveSpeed = 1f;
        else if (isMoving)
            moveSpeed = 0.5f;

        gunAnimator.SetFloat("MoveSpeed", moveSpeed);
    }

    bool IsPlayerMoving()
    {
        // Method 1: Using Character Controller velocity (more accurate)
        if (playerController != null)
        {
            return playerController.velocity.magnitude > 0.1f;
        }

        // Method 2: Fallback - check input directly
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        return Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        currentAmmo--;

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

        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Reload");
        }
    }
}