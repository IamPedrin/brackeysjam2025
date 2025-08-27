using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour
{

    private int damage;
    private float speed;

    private Rigidbody2D rb;

    public void Setup(int newDamage, float newSpeed)
    {
        this.damage = newDamage;
        this.speed = newSpeed;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        rb.linearVelocity = transform.right * speed;
        
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        
        if (hitInfo.CompareTag("Player"))
        {
            PlayerHealth playerHealth = hitInfo.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        if (!hitInfo.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}