using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("Basic Info")]
    public string characterName;
    public GameObject characterPrefab;


    [Header("Stats (0-100)")]
    [Range(0, 100)] public float attack = 50f;
    [Range(0, 100)] public float defence = 50f;
    [Range(0, 100)] public float mobility = 50f;

    [Header("Visual (Optional)")]
    public Sprite characterIcon; 

    [Header("Gameplay Stats (For Later)")]

    public float maxHealth = 100f;      
    public float damageMultiplier = 1f;
    public float moveSpeed = 5f; 
}