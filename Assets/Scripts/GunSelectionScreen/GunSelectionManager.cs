using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GunSelectionManager : MonoBehaviour
{
    [Header("Gun Data")]
    public GunData[] guns;
    private int currentIndex = 0;

    [Header("Spawn Settings")]
    public Transform cameraTransform; // Main Camera reference
    private GameObject currentGunInstance;

    // Local transform values (relative to camera)
    private Vector3 gunLocalPosition = new Vector3(0f, 0f, 1.5f);
    private Vector3 gunLocalScale = new Vector3(1f, 1f, 1f);

    [Header("UI References")]
    public TextMeshProUGUI gunNameText;
    public Image damageBar;
    public Image fireRateBar;
    public Image rangeBar;


    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;
    public Button selectButton;
    public Button backButton;

    [Header("Animation Settings")]
    public bool animateBars = true;
    public float barAnimationSpeed = 0.3f;
    public bool rotateGun = true;
    public float rotationSpeed = 50f; // Degrees per second

    private float currentRotationY = 0f;

    void Start()
    {
        // Auto-find camera if not assigned
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        leftButton.onClick.AddListener(PreviousGun);
        rightButton.onClick.AddListener(NextGun);
        selectButton.onClick.AddListener(SelectGun);
        backButton.onClick.AddListener(BackToLobby);

        DisplayGun(currentIndex);
    }

    void Update()
    {
        Transform acrRifle = FindChildRecursive(cameraTransform, "ACRRifle");

        if(acrRifle != null)
        {
            currentRotationY += rotationSpeed * Time.deltaTime;
            if(rotateGun && currentRotationY >= 360f)
            {
                currentRotationY -= 360f;
            }
            acrRifle.localRotation = Quaternion.Euler(0f, currentRotationY, 0f);
        }
        else
        {
            Debug.Log("Gun not found in Update");
        }

        //Rotate gun continuously around Y axis
        if (rotateGun && currentGunInstance != null)
        {
            currentRotationY += rotationSpeed * Time.deltaTime;

            // Keep rotation between 0-360
            if (currentRotationY >= 360f)
                currentRotationY -= 360f;

            // Apply rotation (only Y axis, X and Z remain 0)
            currentGunInstance.transform.localRotation = Quaternion.Euler(0f, currentRotationY, 0f);
        }
    }

    public void NextGun()
    {
        DestroyPreviousGun(); // Destroy the current gun before switching to the next one

        currentIndex++;
        if (currentIndex >= guns.Length)
            currentIndex = 0;

        DisplayGun(currentIndex);
    }

    public void PreviousGun()
    {
        DestroyPreviousGun(); // Destroy the current gun before switching to the previous one

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = guns.Length - 1;

        DisplayGun(currentIndex);
    }

    void DestroyPreviousGun()
    {
        // Find all children of the Main Camera and destroy them
        foreach (Transform child in cameraTransform)
        {
            Destroy(child.gameObject);
        }
    }

    void DisplayGun(int index)
    {
        // Destroy previous gun
        if (currentGunInstance != null)
        {
            Destroy(currentGunInstance);
        }

        GunData gun = guns[index];

        // Spawn gun as child of camera
        if (gun.gunPrefab != null && cameraTransform != null)
        {
            // Instantiate gun
            currentGunInstance = Instantiate(
                gun.gunPrefab,
                cameraTransform.position, // World position (will be adjusted)
                cameraTransform.rotation  // World rotation (will be adjusted)
            );

            // Make it child of camera
            currentGunInstance.transform.SetParent(cameraTransform);

            // Check if the gun is "KN-44"
            if (gun.gunName == "KN-44")
            {
                // Locate the second "ACRRifle" inside "Armature"
                Transform armatureTransform = FindChildRecursive(currentGunInstance.transform, "Armature");
                Transform armsTransform = FindChildRecursive(currentGunInstance.transform, "arms");
                Transform secondAcrRifleTransform = null;

                if (armatureTransform != null)
                {
                    secondAcrRifleTransform = FindChildRecursive(armatureTransform, "ACRRifle");
                }

                if (armsTransform != null)
                {
                    armsTransform.gameObject.SetActive(false);
                    Debug.Log("arms object disabled");
                }

                if (secondAcrRifleTransform != null)
                {
                    // Reparent the second "ACRRifle" directly under the Main Camera
                    secondAcrRifleTransform.SetParent(cameraTransform);

                    // Adjust its transform
                    secondAcrRifleTransform.localPosition = new Vector3(0f, -0.1f, 2f);
                    secondAcrRifleTransform.localRotation = Quaternion.Euler(25f, 90f, 0f);
                    secondAcrRifleTransform.localScale = new Vector3(150f, 150f, 150f);

                    Debug.Log("Second ACRRifle reparented and adjusted successfully.");
                }
                else
                {
                    Debug.LogError("Second ACRRifle not found inside Armature.");
                }
            }
            else
            {
                // Set LOCAL transform values (relative to camera)
                currentGunInstance.transform.localPosition = gunLocalPosition; // (0, 0, 1.5)
                currentGunInstance.transform.localRotation = Quaternion.Euler(0f, 90f, 0f); // Start at 0
                currentGunInstance.transform.localScale = gunLocalScale; // (1, 1, 1)
            }

            // Reset rotation counter
            currentRotationY = 0f;

            Debug.Log($"Gun '{gun.gunName}' spawned as child of camera with local position {gunLocalPosition}");
        }
        else if (cameraTransform == null)
        {
            Debug.LogError("Camera transform not assigned!");
        }
        else
        {
            Debug.LogError($"Gun prefab is null for {gun.gunName}");
        }

        // Update UI
        if (gunNameText != null)
            gunNameText.text = gun.gunName;

        // Update stat bars
        if (animateBars)
        {
            if (damageBar != null)
                StartCoroutine(AnimateBar(damageBar, gun.damage / 100f));
            if (fireRateBar != null)
                StartCoroutine(AnimateBar(fireRateBar, gun.fireRate / 100f));
            if (rangeBar != null)
                StartCoroutine(AnimateBar(rangeBar, gun.range / 100f));
        }
        else
        {
            if (damageBar != null)
                damageBar.fillAmount = gun.damage / 100f;
            if (fireRateBar != null)
                fireRateBar.fillAmount = gun.fireRate / 100f;
            if (rangeBar != null)
                rangeBar.fillAmount = gun.range / 100f;
        }
    }

    Transform FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform result = FindChildRecursive(child, childName);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    IEnumerator AnimateBar(Image bar, float targetFill)
    {
        float startFill = bar.fillAmount;
        float elapsed = 0f;

        while (elapsed < barAnimationSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / barAnimationSpeed;
            bar.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        bar.fillAmount = targetFill;
    }

    public void SelectGun()
    {
        // Save selected gun
        PlayerPrefs.SetInt("SelectedGun", currentIndex);
        PlayerPrefs.SetString("SelectedGunName", guns[currentIndex].gunName);

        // Save gun stats
        PlayerPrefs.SetFloat("SelectedGunDamage", guns[currentIndex].actualDamage);
        PlayerPrefs.SetFloat("SelectedGunFireRate", guns[currentIndex].actualFireRate);
        PlayerPrefs.SetFloat("SelectedGunRange", guns[currentIndex].actualRange);
        PlayerPrefs.SetInt("SelectedGunAmmo", guns[currentIndex].maxAmmo);

        PlayerPrefs.Save();

        Debug.Log("Selected gun: " + guns[currentIndex].gunName);

        // Return to lobby
        SceneManager.LoadScene("MainLobby");
    }

    public void BackToLobby()
    {
        SceneManager.LoadScene("MainLobby");
    }
}