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

        
        GetComponent<BossAI>().enabled = false;
        Destroy(gameObject, 5f);
    }
}