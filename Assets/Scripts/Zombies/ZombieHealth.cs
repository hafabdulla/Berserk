using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;

    public Animator animator;
    public GameObject hitEffect; 

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (animator != null)
            animator.SetTrigger("Hit"); 
        

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position + Vector3.up * 1f, Quaternion.identity);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Zombie killed!");

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 50f; 
        rb.AddForce(Vector3.back * 2f, ForceMode.Impulse); 


        ZombieController ai = GetComponent<ZombieController>();
        if (ai != null)
            ai.enabled = false;

        // Notify GameManager (Level 2 objective)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyKilled();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null!");
        }


        Destroy(gameObject, 3f);
    }
}
