using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("A referência para o Scriptable Object com os stats do chefe.")]
    public BossStats bossStats;
    
    [Header("Referências da UI")]
    [Tooltip("A imagem de preenchimento da barra de vida do chefe.")]
    public Image healthBarFill;

    private float currentHealth;

    void Start()
    {
        if (bossStats == null)
        {
            Debug.LogError("BossStats não está atribuído no BossHealth de " + gameObject.name);
            return;
        }
        
        // Configura a vida inicial diretamente do BossStats
        currentHealth = bossStats.maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, bossStats.maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / bossStats.maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log("O CHEFE FOI DERROTADO!");
        // --- LÓGICA DE VITÓRIA AQUI ---
        // - Pare todos os ataques (desative o script BossAI)
        // - Toque uma animação de morte épica
        // - Instancie efeitos visuais e sonoros de explosão
        // - Drope loot especial
        // - Chame uma função no seu GameManager para registrar a vitória
        // - Pare a música da batalha e toque uma música de vitória
        
        GetComponent<BossAI>().enabled = false; // Desativa a IA para parar os ataques
        // Adicione aqui sua animação de morte
        Destroy(gameObject, 5f); // Destrói o objeto do chefe após 5 segundos
    }
}