using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerProjectile : MonoBehaviour
{

    private float speed;
    private int damage; 

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

        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        if (!hitInfo.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}