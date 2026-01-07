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
    private bool isInspecting = false;

    private float nextTimeToFire = 0f;

    [Header("Recoil Settings")]
    public float recoilAmount = 1.0f;
    public float recoilRecoverySpeed = 2.0f;
    public float maxRecoilAngle = 5.0f;

    private Vector3 currentRecoil = Vector3.zero;
    private Vector3 targetRecoil = Vector3.zero;

    GameObject audioManager;
    [Header("Head Light Settings")]
    public GameObject headLamp;//assign in inspector 
    private bool lightstate = true;
    private PlayerStats playerStats;

    private bool wasRunning = false; // Add this field to cache running state

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
        headLampCheck();
    }

    void HandleMovementAnimations()
    {
        if (gunAnimator == null) return;

        bool isMoving = IsPlayerMoving();
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift);
        
        // Determine running state with hysteresis to prevent flickering
        bool isRunning;
        if (wasRunning)
        {
            // If we were running, stay running as long as shift is held (even if movement briefly dips)
            isRunning = shiftHeld && isMoving;
        }
        else
        {
            // If we weren't running, start running only if both conditions are met
            isRunning = shiftHeld && isMoving;
        }
        wasRunning = isRunning;

        // Set bools in correct order - set IsRunning first to prevent transition conflicts
        if (isRunning)
        {
            gunAnimator.SetBool("IsRunning", true);
            gunAnimator.SetBool("IsWalking", false);
        }
        else if (isMoving)
        {
            gunAnimator.SetBool("IsRunning", false);
            gunAnimator.SetBool("IsWalking", true);
        }
        else
        {
            gunAnimator.SetBool("IsRunning", false);
            gunAnimator.SetBool("IsWalking", false);
        }

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
        bool isDoingSomething = isMoving || isShooting || isReloading;

        // If player is doing something, reset idle timer and interrupt inspection if active
        if (isDoingSomething)
        {
            idleTimer = 0f;

            // Interrupt inspect animation if currently inspecting
            if (isInspecting)
            {
                isInspecting = false;
                // Reset the Inspect trigger to stop the animation
                gunAnimator.ResetTrigger("Inspect");
                // Force transition back to idle/movement state
                gunAnimator.SetTrigger("CancelInspect");
            }
            return;
        }

        // Player is idle - increment idle timer
        idleTimer += Time.deltaTime;

        // Trigger inspect animation when idle long enough and not already inspecting
        if (idleTimer >= idleTimeBeforeInspect && !isInspecting)
        {
            gunAnimator.SetTrigger("Inspect");
            isInspecting = true;
        }
    }

    // Call this from an Animation Event at the end of the Inspect animation
    public void OnInspectAnimationComplete()
    {
        isInspecting = false;
        idleTimer = 0f; // Reset timer so inspect can play again after another idle period
    }

    bool IsPlayerMoving()
    {
        // Use GetAxisRaw for immediate input values without smoothing
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // GetAxisRaw returns -1, 0, or 1 immediately, so any non-zero value means moving
        if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
        {
            return true;
        }

        return false;
    }

    public void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        currentAmmo--;

        // Reset idle animation logic and interrupt inspection if active
        idleTimer = 0f;
        if (isInspecting)
        {
            isInspecting = false;
            gunAnimator.ResetTrigger("Inspect");
            gunAnimator.SetTrigger("CancelInspect");
        }

        // FX
        muzzleFlash.Play();
        audioManager.GetComponent<AudioController>().playPlasmaGunSound();

        if (gunAnimator != null)
            gunAnimator.SetTrigger("Shoot");

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.root.name);

            ZombieHealth zombieTarget = hit.transform.GetComponentInParent<ZombieHealth>();

            if (zombieTarget != null)
            {
                float actualDamage = damage;
                if (playerStats != null)
                    actualDamage *= playerStats.GetDamageMultiplier();

                zombieTarget.TakeDamage(100);
                SpawnImpact(hit);
                return;
            }

            CrocMenHealth crocMenTarget = hit.transform.GetComponentInParent<CrocMenHealth>();
            if (crocMenTarget != null)
            {
                float actualDamage = damage;
                if (playerStats != null)
                    actualDamage *= playerStats.GetDamageMultiplier();

                crocMenTarget.TakeDamage(100);
                SpawnImpact(hit);
                return;
            }

            Cyber2Health cyber2Target = hit.transform.GetComponentInParent<Cyber2Health>();
            if (cyber2Target != null)
            {
                float actualDamage = damage;
                if (playerStats != null)
                    actualDamage *= playerStats.GetDamageMultiplier();

                cyber2Target.TakeDamage(100);
                SpawnImpact(hit);
                return;
            }

            // Start with the direct hit object
            Transform root = hit.transform;

            // If the child wasn't tagged, check the parent root
            if (!root.CompareTag("Target_To_BeDestroyed"))
                root = hit.transform.root;

            //  If the object (or its parent) IS the target
            if (root.CompareTag("Target_To_BeDestroyed"))
            {
                EnemyHealth target = root.GetComponent<EnemyHealth>();

                if (target != null)
                {
                    float actualDamage = damage;

                    if (playerStats != null)
                        actualDamage = damage * playerStats.GetDamageMultiplier();

                    target.TakeDamage(actualDamage);
                }

                // Impact FX
                SpawnImpact(hit);
                return;
            }

            // Not target → normal impact only
            SpawnImpact(hit);
        }
        else
        {
            Debug.Log("Raycast hit NOTHING");
        }
    }

    private void SpawnImpact(RaycastHit hit)
    {
        if (impactEffect != null)
        {
            Vector3 pos = hit.point + hit.normal * 0.02f;
            Quaternion rot = Quaternion.LookRotation(hit.normal);
            Instantiate(impactEffect, pos, rot);
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

    void headLampCheck()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            lightstate = !lightstate;
            Debug.Log("Headlamp toggled");
            headLamp.SetActive(lightstate);
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

        // Reset idle timer and interrupt inspection if active
        idleTimer = 0f;
        if (isInspecting)
        {
            isInspecting = false;
            gunAnimator.ResetTrigger("Inspect");
            gunAnimator.SetTrigger("CancelInspect");
        }

        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Reload");
        }
    }
}