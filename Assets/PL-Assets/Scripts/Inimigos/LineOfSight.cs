using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public EnemyStats stats;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    public bool CanSeePlayer()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, stats.lineOfSightRadius, playerLayer);

        foreach (var hitCollider in hitColliders)
        {
            Transform player = hitCollider.transform;
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (!Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer))
            {
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, stats.lineOfSightRadius);
    }
}
