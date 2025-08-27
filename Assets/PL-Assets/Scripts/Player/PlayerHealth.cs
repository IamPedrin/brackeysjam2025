using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuração de Stats")]
    public PlayerStats stats;

    [Header("Configuração da UI de Corações")]
    public GameObject heartPrefab;
    public Transform heartsParent;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    private int currentHealth;
    private List<Image> heartImages = new List<Image>();

    void Start()
    {
        currentHealth = stats.maxHearts;
        GenerateHeartsUI();
        UpdateHealthUI();
    }

    private void GenerateHeartsUI()
    {
        foreach (Transform child in heartsParent)
        {
            Destroy(child.gameObject);
        }
        heartImages.Clear();

        for (int i = 0; i < stats.maxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsParent);
            heartImages.Add(heart.GetComponent<Image>());
        }
    }

    private void UpdateHealthUI()
    {
        
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < currentHealth)
            {
                
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }

    
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, stats.maxHearts);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, stats.maxHearts);
        UpdateHealthUI();
    }

    private void Die()
    {
        Debug.Log("morreu");
        Destroy(gameObject);
    }
}
