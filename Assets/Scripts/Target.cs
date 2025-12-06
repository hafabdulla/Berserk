using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;   // tweak however you want
    public GameObject fireEffect; // drag your explosion/fire prefab here

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
            Instantiate(fireEffect, transform.position, Quaternion.identity);

        // Destroy the bomb object
        Destroy(gameObject);

        // TODO: Trigger Game Won panel here

        Debug.Log("Bomb destroyed — You win!");
    }
}
