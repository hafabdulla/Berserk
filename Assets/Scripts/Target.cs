using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;   // tweak however you want
    public GameObject fireEffect; 


    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Spawn fire/explosion effect
        if (fireEffect != null)
        {
            Vector3 explosionPos = new Vector3(-15.6f, 0.69f, 182.61f);
            Instantiate(fireEffect, explosionPos, Quaternion.identity);
        }

        Debug.Log("Target destroyed!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTargetDestroyed();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null!");
        }

        Destroy(gameObject);
    }
}
