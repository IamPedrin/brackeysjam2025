// Scripts/PlayerCombat.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Configuração de Stats")]
    public PlayerStats stats;

    [Header("Pontos de Disparo Direcionais")]
    public Transform firePointUp;
    public Transform firePointDown;
    public Transform firePointLeft;
    public Transform firePointRight;

    private float attackCooldown;
    private float nextAttackTime = 0f;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (stats != null)
        {
            attackCooldown = 1f / stats.attackSpeed;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= nextAttackTime)
        {
            Shoot();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Shoot()
    {
        if (stats.projectilePrefab == null || playerController == null) return;

        Vector2 rawDirection = playerController.LastMoveDirection;
        Vector2 shootDirection;

        if (Mathf.Abs(rawDirection.x) > Mathf.Abs(rawDirection.y))
        {
            shootDirection = new Vector2(Mathf.Sign(rawDirection.x), 0);
        }
        else
        {
            shootDirection = new Vector2(0, Mathf.Sign(rawDirection.y));
        }

        Transform selectedFirePoint = SelectFirePoint(shootDirection);

        if (selectedFirePoint == null)
        {
            Debug.LogError("Nenhum Fire Point válido encontrado para a direção: " + shootDirection);
            return;
        }

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        
        GameObject projectileGO = Instantiate(stats.projectilePrefab, selectedFirePoint.position, rotation);
        
        PlayerProjectile projectile = projectileGO.GetComponent<PlayerProjectile>();
        if (projectile != null)
        {
            float finalDamage = stats.projectileDamage * stats.damageMultiplier;
            projectile.Setup((int)finalDamage, stats.projectileSpeed);
        }
    }

    private Transform SelectFirePoint(Vector2 direction)
    {
        if (direction == Vector2.up) return firePointUp;
        if (direction == Vector2.down) return firePointDown;
        if (direction == Vector2.left) return firePointLeft;
        if (direction == Vector2.right) return firePointRight;
        
        return firePointRight;
    }
}