using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyStats stats;
    public GameObject cartaPrefab;
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
        if (cartaPrefab && stats != null && Random.value < stats.cartaDropChance)
        {
            var carta = Instantiate(cartaPrefab, transform.position, Quaternion.identity);
            var cartaPickup = carta.GetComponent<TF_CartaPickup>();
            if (cartaPickup != null)
                cartaPickup.cartaTipo = stats.cartaDropType;
        }
        var pickups = GetComponent<TF_bichusdineroepickups>();
        if (pickups != null)
            pickups.OnEnemyDefeated();
        Destroy(gameObject);
    }
}
