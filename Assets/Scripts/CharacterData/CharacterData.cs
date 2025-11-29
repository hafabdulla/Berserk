using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    public GameObject characterPrefab; // The 3D model

    [Header("Stats (0-100)")]
    [Range(0, 100)] public float attack = 50f;    // ATTACK bar (red)
    [Range(0, 100)] public float defence = 50f;   // DEFENCE bar (blue)
    [Range(0, 100)] public float mobility = 50f;  // MOBILITY bar (yellow/green)

    [Header("Visual (Optional)")]
    public Sprite characterIcon; // For UI thumbnails if needed later

    [Header("Gameplay Stats (For Later)")]
    // These will be used in actual gameplay
    public float maxHealth = 100f;        // Based on defence
    public float damageMultiplier = 1f;   // Based on attack
    public float moveSpeed = 5f;          // Based on mobility
}