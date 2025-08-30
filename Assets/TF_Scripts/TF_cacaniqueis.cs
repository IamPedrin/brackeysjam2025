using UnityEngine;

public class TF_cacaniqueis : MonoBehaviour
{
    public enum BetAmount { Baixa = 500, Media = 1000, Alta = 2000 }

    public void Apostar(int fichas)
    {
        var player = GameObject.FindWithTag("Player");
        var playerStats = player?.GetComponent<PlayerStats>();
        if (playerStats == null || playerStats.chips < fichas) return;
        playerStats.chips -= fichas;

        float roll = Random.value;

        if (roll < 0.4f) // 40% Buff Comum
        {
            AplicarBuffComum(playerStats, fichas);
        }
        else if (roll < 0.5f) // 10% Buff Raro
        {
            AplicarBuffComum(playerStats, fichas);
            AplicarBuffRaro(playerStats, fichas);
        }
        else if (roll < 0.9f) // 40% Debuff Comum
        {
            AplicarDebuffComum(playerStats, fichas);
        }
        else // 10% Debuff Raro
        {
            AplicarDebuffComum(playerStats, fichas);
            AplicarDebuffRaro(playerStats, fichas);
        }
    }

    void AplicarBuffComum(PlayerStats stats, int fichas)
    {
        // Dobro de dano e ganha um coração a cada 15 segundos
        float duration = Mathf.Lerp(10, 60, fichas / 2000f); // Example scaling
        stats.damageMultiplier = 2f;
        // TODO: Implement heart gain every 15s for duration
        Debug.Log($"Buff Comum: Dano x2, coração a cada 15s por {duration}s");
    }

    void AplicarBuffRaro(PlayerStats stats, int fichas)
    {
        // Invulnerabilidade
        float duration = Mathf.Lerp(5, 30, fichas / 2000f); // Example scaling
        stats.isInvulnerable = true;
        Debug.Log($"Buff Raro: Invulnerável por {duration}s");
    }

    void AplicarDebuffComum(PlayerStats stats, int fichas)
    {
        // -50% velocidade, recebe o dobro de dano
        float duration = Mathf.Lerp(10, 60, fichas / 2000f);
        stats.speedMultiplier = 0.5f;
        stats.damageTakenMultiplier = 2f;
        Debug.Log($"Debuff Comum: Velocidade -50%, dano recebido x2 por {duration}s");
    }

    void AplicarDebuffRaro(PlayerStats stats, int fichas)
    {
        // Não pode usar Copas, -50% de dano
        float duration = Mathf.Lerp(5, 30, fichas / 2000f);
        var player = GameObject.FindWithTag("Player");
        var cartas = player?.GetComponent<TF_Cartas>();
        if (cartas != null)
            cartas.canUseCopas = false;
        stats.damageMultiplier = 0.5f;
        Debug.Log($"Debuff Raro: Sem Copas, dano x0.5 por {duration}s");
    }
}
