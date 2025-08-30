using UnityEngine;

public class TF_cacaniqueis : MonoBehaviour
{
    private System.Random rng;
    private PlayerHealth playerHealth;
    private PlayerStats playerStats;
    private TF_Cartas cartas;

    void Awake()
    {
        GameObject geracaoObj = GameObject.FindWithTag("Geracao");
        if (geracaoObj != null)
        {
            var seedProvider = geracaoObj.GetComponent<TF_Generation>();
            if (seedProvider != null)
            {
                rng = new System.Random(seedProvider.seed);
            }
            else
            {
                rng = new System.Random();
            }
        }
        else
        {
            rng = new System.Random();
        }
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerStats = playerHealth.stats;
            cartas = player.GetComponent<TF_Cartas>();
        }
    }

    public void Apostar(int fichas)
    {
        if (playerStats == null || playerStats.chips < fichas)
            return;
        playerStats.chips -= fichas;
        float roll = (float)rng.NextDouble();
        if (roll < 0.4f)
            AplicarBuffComum(playerStats, fichas);
        else if (roll < 0.5f)
        {
            AplicarBuffComum(playerStats, fichas);
            AplicarBuffRaro(playerStats, fichas);
        }
        else if (roll < 0.9f)
            AplicarDebuffComum(playerStats, fichas);
        else
        {
            AplicarDebuffComum(playerStats, fichas);
            AplicarDebuffRaro(playerStats, fichas);
        }
    }

    void AplicarBuffComum(PlayerStats stats, int fichas)
    {
        float duration = Mathf.Lerp(10, 60, fichas / 2000f);
        stats.damageMultiplier = 2f;
        StartCoroutine(RemoverBuffComum(stats, duration));
    }

    System.Collections.IEnumerator RemoverBuffComum(PlayerStats stats, float duration)
    {
        yield return new WaitForSeconds(duration);
        stats.damageMultiplier = 1f;
    }

    void AplicarBuffRaro(PlayerStats stats, int fichas)
    {
        float duration = Mathf.Lerp(5, 30, fichas / 2000f);
        stats.isInvulnerable = true;
        StartCoroutine(RemoverBuffRaro(stats, duration));
    }

    System.Collections.IEnumerator RemoverBuffRaro(PlayerStats stats, float duration)
    {
        yield return new WaitForSeconds(duration);
        stats.isInvulnerable = false;
    }

    void AplicarDebuffComum(PlayerStats stats, int fichas)
    {
        float duration = Mathf.Lerp(10, 60, fichas / 2000f);
        stats.speedMultiplier = 0.5f;
        stats.damageTakenMultiplier = 2f;
        StartCoroutine(RemoverDebuffComum(stats, duration));
    }

    System.Collections.IEnumerator RemoverDebuffComum(PlayerStats stats, float duration)
    {
        yield return new WaitForSeconds(duration);
        stats.speedMultiplier = 1f;
        stats.damageTakenMultiplier = 1f;
    }

    void AplicarDebuffRaro(PlayerStats stats, int fichas)
    {
        float duration = Mathf.Lerp(5, 30, fichas / 2000f);
        if (cartas != null)
            cartas.canUseCopas = false;
        stats.damageMultiplier = 0.5f;
        StartCoroutine(RemoverDebuffRaro(stats, cartas, duration));
    }

    System.Collections.IEnumerator RemoverDebuffRaro(PlayerStats stats, TF_Cartas cartas, float duration)
    {
        yield return new WaitForSeconds(duration);
        stats.damageMultiplier = 1f;
        if (cartas != null)
            cartas.canUseCopas = true;
    }
}
