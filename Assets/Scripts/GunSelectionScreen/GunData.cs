using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Weapons/Gun Data")]
public class GunData : ScriptableObject
{
    [Header("Basic Info")]
    public string gunName;
    public GameObject gunPrefab; // The 3D model

    [Header("Display Stats (0-100)")]
    [Range(0, 100)] public float damage = 50f;
    [Range(0, 100)] public float fireRate = 50f;
    [Range(0, 100)] public float range = 50f;
    [Range(0, 100)] public float ammo = 50f;

    [Header("Actual Gameplay Values")]
    public float actualDamage = 25f;      // Real damage per shot
    public float actualFireRate = 0.1f;   // Time between shots
    public float actualRange = 100f;      // Max shooting distance
    public int maxAmmo = 30;              // Magazine size

    [Header("Visual (Optional)")]
    public Sprite gunIcon;
}
