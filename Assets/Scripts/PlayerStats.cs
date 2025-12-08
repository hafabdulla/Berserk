using UnityEngine;
using StarterAssets; // For Starter Assets FirstPersonController
public class PlayerStats : MonoBehaviour
{
    [Header("Character Stats (from selection)")]
    public float attack = 50f;
    public float defence = 50f;
    public float mobility = 50f;
    [Header("Gameplay Values")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float damageMultiplier = 1f;
    private FirstPersonController fpsController;
    void Start()
    {
        LoadSelectedCharacterStats();
        ApplyStatsToGameplay();
    }
    void LoadSelectedCharacterStats()
    {
        // Load stats saved from character selection
        attack = PlayerPrefs.GetFloat("SelectedCharacterAttack", 50f);
        defence = PlayerPrefs.GetFloat("SelectedCharacterDefence", 50f);
        mobility = PlayerPrefs.GetFloat("SelectedCharacterMobility", 50f);
        string characterName = PlayerPrefs.GetString("SelectedCharacterName", "Default");
        Debug.Log($"Playing as: {characterName}");
        Debug.Log($"Stats - Attack:{attack} Defence:{defence} Mobility:{mobility}");
    }
    void ApplyStatsToGameplay()
    {
        fpsController = GetComponent<FirstPersonController>();

        maxHealth = 50f + (defence * 1.5f);
        currentHealth = maxHealth;

        damageMultiplier = 0.5f * (attack / 100f);

        if(fpsController != null)
        {
            float speedBoost = mobility / 50f;
            fpsController.MoveSpeed *= speedBoost;
            fpsController.SprintSpeed *= speedBoost;

            Debug.Log($"Movement speed adjusted: {fpsController.MoveSpeed}");
        }
        Debug.Log($"Applied - MaxHP:{maxHealth} DamageMult:{damageMultiplier}x");
    }

    public float GetDamageMultiplier()
    {
        return damageMultiplier;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Player took {damage} damage. HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("No PauseManager found in scene!");
        }
    }

}
