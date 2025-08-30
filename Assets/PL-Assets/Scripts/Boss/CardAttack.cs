using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardAttack : MonoBehaviour
{
    [Tooltip("Tempo em segundos que o ataque permanece ativo na cena antes de desaparecer.")]
    public float lifetime = 1.5f;

    private int damage;


    public void Setup(int damageAmount)
    {
        this.damage = damageAmount;
    }

    void Start()
    {

        GetComponent<Collider2D>().isTrigger = true;
        

        Destroy(gameObject, lifetime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {

                playerHealth.TakeDamage(damage);
            }

            GetComponent<Collider2D>().enabled = false;
        }
    }
}