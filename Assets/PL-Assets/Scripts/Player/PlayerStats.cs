using UnityEngine;

[CreateAssetMenu(fileName = "New Player Stats", menuName = "Player/Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Vida em Corações")]
    public int maxHearts = 3;


    [Header("Movimento")]
    public float moveSpeed = 5f;

    [Header("Combate")]
    public float attackSpeed = 2f;
    public float projectileDamage = 1f;
    public float projectileSpeed = 15f;
    public GameObject projectilePrefab;
}