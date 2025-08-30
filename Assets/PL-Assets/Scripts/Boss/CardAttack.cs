using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CardAttack : MonoBehaviour
{
    [Tooltip("Tempo em segundos que o ataque permanece ativo na cena antes de desaparecer.")]
    public float lifetime = 1.5f;

    private int damage;

    /// <summary>
    /// Configura o dano do ataque. Este método é chamado pelo BossAI no momento da criação.
    /// </summary>
    public void Setup(int damageAmount)
    {
        this.damage = damageAmount;
    }

    void Start()
    {
        // Garante que o collider é um trigger para não ter colisões físicas
        GetComponent<Collider2D>().isTrigger = true;
        
        // Destrói este objeto de ataque após o tempo de vida definido
        Destroy(gameObject, lifetime);
    }

    // Este método é chamado quando outro collider entra na área do trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou é o jogador
        if (other.CompareTag("Player"))
        {
            // Tenta pegar o componente de vida do jogador
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Aplica o dano
                playerHealth.TakeDamage(damage);
            }
            
            // Desativa o collider imediatamente após o primeiro acerto
            // para evitar que o mesmo ataque cause dano várias vezes.
            GetComponent<Collider2D>().enabled = false;
        }
    }
}