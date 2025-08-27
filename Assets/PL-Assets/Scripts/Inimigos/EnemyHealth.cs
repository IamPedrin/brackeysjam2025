using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyStats stats;
    private float currentHealth;

    void Start()
    {
        currentHealth = stats.health;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
