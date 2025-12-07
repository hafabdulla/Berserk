using UnityEngine;

public class CrocMenHealth : MonoBehaviour
{
    public float maxHealth = 200f;
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
            animator.SetTrigger("Hit"); // hit animation

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position + Vector3.up * 1f, Quaternion.identity);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Make the enemy fall
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 50f; // heavy so it looks like it collapses
        rb.AddForce(Vector3.back * 2f, ForceMode.Impulse); // small push backward

        // Stop AI movement
        ZombieController ai = GetComponent<ZombieController>();
        if (ai != null)
            ai.enabled = false;

        // Remove after some seconds
        Destroy(gameObject, 3f);
    }
}
