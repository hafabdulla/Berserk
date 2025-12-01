using UnityEngine;

public class PlayerGunLoader : MonoBehaviour
{
    [Header("Gun Prefabs")]
    public GameObject[] allGunPrefabs; // All guns in same order as GunData assets

    [Header("Gun Parent")]
    public Transform gunParent; // The camera where gun should attach

    void Start()
    {
        LoadSelectedGun();
    }

    void LoadSelectedGun()
    {
        if (!PlayerPrefs.HasKey("SelectedGun"))
        {
            Debug.LogWarning("No gun selected! Using default.");
            if (allGunPrefabs.Length > 0)
                SpawnGun(0);
            return;
        }

        int selectedIndex = PlayerPrefs.GetInt("SelectedGun");
        SpawnGun(selectedIndex);
    }

    void SpawnGun(int index)
    {
        if (index < 0 || index >= allGunPrefabs.Length)
        {
            Debug.LogError("Invalid gun index: " + index);
            return;
        }

        // Spawn gun
        GameObject gun = Instantiate(
            allGunPrefabs[index],
            gunParent.position,
            gunParent.rotation,
            gunParent
        );

        // Position gun correctly
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;

        // Load gun stats
        WeaponController weaponController = gun.GetComponent<WeaponController>();
        if (weaponController != null)
        {
            weaponController.damage = PlayerPrefs.GetFloat("SelectedGunDamage", 25f);
            weaponController.fireRate = PlayerPrefs.GetFloat("SelectedGunFireRate", 0.1f);
            weaponController.range = PlayerPrefs.GetFloat("SelectedGunRange", 100f);
            weaponController.maxAmmo = PlayerPrefs.GetInt("SelectedGunAmmo", 30);
            weaponController.currentAmmo = weaponController.maxAmmo;

            Debug.Log($"Loaded gun: {PlayerPrefs.GetString("SelectedGunName")} - Damage:{weaponController.damage}");
        }
    }
}