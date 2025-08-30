using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Stats", menuName = "Boss/Stats")]
public class BossStats : ScriptableObject
{
    [Header("Estatísticas Principais")]
    public float maxHealth = 1000f;

    [Header("Pacing da Batalha (Timers)")]
    public float idleDuration = 3f;
    public float cardAttackTelegraphDuration = 2f;
    public float cardAttackActiveDuration = 1f;

    [Header("Ataque de Cartas")]
    public int numberOfCardAttacks = 5;
    public int cardAttackDamage = 2;
    public GameObject warningIndicatorPrefab;
    public GameObject cardAttackPrefab;

    [Header("Ataque de Moedas")]
    public int numberOfCoinsToThrow = 3;
    public float timeBetweenCoins = 0.3f;
    public int coinAttackDamage = 1;
    public float coinProjectileSpeed = 10f;
    public GameObject coinProjectilePrefab;
}