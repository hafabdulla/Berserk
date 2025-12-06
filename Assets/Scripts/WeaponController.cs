using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public int maxAmmo = 30;
    public int currentAmmo;

    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Animation")]
    public Animator gunAnimator;

    [Header("Movement Detection")]
    public CharacterController playerController;

    [Header("Idle Inspection")]
    public float idleTimeBeforeInspect = 5f; // Time in seconds before inspect plays
    private float idleTimer = 0f;
    private bool hasPlayedInspect = false;

    private float nextTimeToFire = 0f;

    [Header("Recoil Settings")]
    public float recoilAmount = 1.0f;
    public float recoilRecoverySpeed = 2.0f;
    public float maxRecoilAngle = 5.0f;

    private Vector3 currentRecoil = Vector3.zero;
    private Vector3 targetRecoil = Vector3.zero;

    GameObject audioManager;


    private PlayerStats playerStats;
    void Start()
    {
        currentAmmo = maxAmmo;

        if (fpsCam == null)
        {
            fpsCam = Camera.main;
        }

        if (playerController == null)
        {
            playerController = GetComponentInParent<CharacterController>();
        }


        playerStats = GetComponentInParent<PlayerStats>();

        audioManager = GameObject.Find("AudioManager");
    }

    void Update()
    {
        // Handle movement animations
        HandleMovementAnimations();

        // Handle idle inspection
        HandleIdleInspection();

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

        HandleRecoil();
    }

    void HandleMovementAnimations()
    {
        if (gunAnimator == null) return;

        bool isMoving = IsPlayerMoving();
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;

        gunAnimator.SetBool("IsWalking", isMoving && !isRunning);
        gunAnimator.SetBool("IsRunning", isRunning);

        float moveSpeed = 0f;
        if (isRunning)
            moveSpeed = 1f;
        else if (isMoving)
            moveSpeed = 0.5f;

        gunAnimator.SetFloat("MoveSpeed", moveSpeed);
    }

    void HandleIdleInspection()
    {
        if (gunAnimator == null) return;

        bool isMoving = IsPlayerMoving();
        bool isShooting = Input.GetButton("Fire1");
        bool isReloading = Input.GetKey(KeyCode.R);

        // Reset timer if player is doing something
        if (isMoving || isShooting || isReloading)
        {
            idleTimer = 0f;
            hasPlayedInspect = false;
            return;
        }

        // Increment idle timer
        idleTimer += Time.deltaTime;

        // Trigger inspect animation when idle long enough
        if (idleTimer >= idleTimeBeforeInspect && !hasPlayedInspect)
        {
            gunAnimator.SetTrigger("Inspect");
            hasPlayedInspect = true;
            idleTimer = 0f; // Reset timer after playing
        }
    }

    bool IsPlayerMoving()
    {
        if (playerController != null)
        {
            return playerController.velocity.magnitude > 0.1f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        return Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
    }

    public void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        currentAmmo--;

        // Reset idle timer when shooting
        idleTimer = 0f;
        hasPlayedInspect = false;

        muzzleFlash.Play();
        audioManager.GetComponent<AudioController>().playPlasmaGunSound();

        if (gunAnimator != null)
            gunAnimator.SetTrigger("Shoot");

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target != null)
            {
                // Calculate actual damage with character stats
                float actualDamage = damage;

                if (playerStats != null)
                {
                    actualDamage = damage * playerStats.GetDamageMultiplier();
                    Debug.Log($"Damage: {damage} x {playerStats.GetDamageMultiplier()} = {actualDamage}");
                }

                target.TakeDamage(actualDamage); // Use multiplied damage
            }

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
        fpsCam.transform.localRotation = Quaternion.Euler(currentRecoil);

        if (currentRecoil != Vector3.zero)
        {
            currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
        }
    }

    void AddRecoil()
    {
        float verticalRecoil = Random.Range(0.5f, 1.0f) * recoilAmount;
        float horizontalRecoil = Random.Range(-0.5f, 0.5f) * recoilAmount;

        Vector3 newRecoil = new Vector3(-verticalRecoil, horizontalRecoil, 0);
        currentRecoil += newRecoil;

        currentRecoil.x = Mathf.Clamp(currentRecoil.x, -maxRecoilAngle, maxRecoilAngle);
        currentRecoil.y = Mathf.Clamp(currentRecoil.y, -maxRecoilAngle, maxRecoilAngle);
    }

    void Reload()
    {
        Debug.Log("Reloading...");
        currentAmmo = maxAmmo;

        // Reset idle timer when reloading
        idleTimer = 0f;
        hasPlayedInspect = false;

        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Reload");
        }
    }
}