using UnityEngine;

public enum AttackType { Melee, Ranged }



[CreateAssetMenu(fileName = "New Enemy Stats", menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 4f;

    [Header("Vitals")]
    public float health = 100f;

    [Header("AI")]
    public float lineOfSightRadius = 5f;
    public float aggroDuration = 3f; // Tempo que o inimigo continua perseguindo após perder o jogador de vista

    [Header("Combat")]
    public AttackType attackType;
    public float attackRange = 1f;
    public int attackDamage = 1;
    public float attackCooldown = 1.5f; // Tempo entre ataques

    [Header("Ranged")]
    public GameObject projectilePrefab; // Ranged
    public float projectileSpeed = 8f; //  Ranged

    [Header("Drops")]
    public int chipsDrop = 100;
    public TF_Cartas.Cartas cartaDropType = TF_Cartas.Cartas.Paus;
    [Range(0f, 1f)] public float cartaDropChance = 0.2f;

}
