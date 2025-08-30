using UnityEngine;

[CreateAssetMenu(fileName = "New Player Stats", menuName = "Player/Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Vida em Corações")]
    public int maxHearts = 3;


    [Header("Movimento")]
    public float moveSpeed = 5f;
    public float speedMultiplier = 1f;

    [Header("Combate")]
    public float attackSpeed = 2f;
    public float projectileDamage = 1f;
    public float projectileSpeed = 15f;
    public GameObject projectilePrefab;

    [Header("Fichas")]
    public int chips = 0;

    [Header("Multiplicadores e Status Temporários")]
    public float damageMultiplier = 1f;
    public float damageTakenMultiplier = 1f;
    public float chipGainMultiplier = 1f;

    public bool isInvulnerable = false;
}