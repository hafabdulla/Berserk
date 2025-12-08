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
        {
            Vector3 explosionPos = new Vector3(-15.6f, 0.69f, 182.61f);
            Instantiate(fireEffect, explosionPos, Quaternion.identity);
        }
        // Destroy the bomb object
        Destroy(gameObject);

        // TODO: Trigger Game Won panel here

        Debug.Log("Bomb destroyed — You win!");
        GameManager pm = FindObjectOfType<GameManager>();
        if (pm != null)
        {
            pm.ShowLevelComplete();
        }
    }
}
