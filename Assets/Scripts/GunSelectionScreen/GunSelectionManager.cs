using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GunSelectionManager : MonoBehaviour
{
    [Header("Gun Data")]
    public GunData[] guns; // CHANGED: was characters
    private int currentIndex = 0;

    [Header("Spawn Settings")]
    private GameObject currentGunInstance;

    // Specific transform values for gun display
    private Vector3 gunPosition = new Vector3(401.8161f, 440.91f, 240.999f);
    private Vector3 gunRotation = new Vector3(-105.594f, 38.82899f, 27.48f);
    private Vector3 gunScale = new Vector3(900f, 900f, 900f);

    [Header("UI References")]
    public TextMeshProUGUI gunNameText; // CHANGED: was characterNameText
    public Image damageBar; // CHANGED: was attackBar
    public Image fireRateBar; // NEW
    public Image rangeBar; // CHANGED: was mobilityBar
    //public Image ammoBar; // NEW (or replace defenceBar)

    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;
    public Button selectButton;
    public Button backButton; // NEW: add back button

    [Header("Animation Settings")]
    public bool animateBars = true;
    public float barAnimationSpeed = 0.3f;
    public bool rotateGun = true;
    public float rotationSpeed = 30f;
    public Vector3 rotationAxis = Vector3.up; // Y axis rotation

    void Start()
    {
        leftButton.onClick.AddListener(PreviousGun); // CHANGED
        rightButton.onClick.AddListener(NextGun); // CHANGED
        selectButton.onClick.AddListener(SelectGun); // CHANGED
        backButton.onClick.AddListener(BackToLobby); // NEW

        DisplayGun(currentIndex); // CHANGED
    }

    void Update()
    {
        // Rotate gun for display
        if (rotateGun && currentGunInstance != null)
        {
            currentGunInstance.transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    public void NextGun()
    {
        currentIndex++;
        if (currentIndex >= guns.Length)
            currentIndex = 0;

        DisplayGun(currentIndex);
    }


    public void PreviousGun()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = guns.Length - 1;

        DisplayGun(currentIndex);
    }

    void DisplayGun(int index)
    {
        if (currentGunInstance != null)
            Destroy(currentGunInstance);

        GunData gun = guns[index];

        // Spawn gun at specific transform
        if (gun.gunPrefab != null)
        {
            currentGunInstance = Instantiate(
                gun.gunPrefab,
                gunPosition,
                Quaternion.Euler(gunRotation)
            );

            // Set scale
            currentGunInstance.transform.localScale = gunScale;

            Debug.Log($"Gun '{gun.gunName}' spawned at {gunPosition}");
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
    public void SelectGun() // CHANGED: was SelectCharacter
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