using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrolling,
    Chasing,
    Attacking
}

public class EnemyAI : MonoBehaviour
{
    public EnemyStats stats;
    public Transform[] patrolPoints;
    public float waitTimeAtPatrolPoint = 1f;
    public LayerMask playerLayer;

    private EnemyState currentState;
    private LineOfSight lineOfSight;
    private Transform player;
    private Rigidbody2D rb;

    [Header("Patrulha")]
    private int currentPatrolIndex = 0;
    private float patrolWaitTimer;

    [Header("Aggro")]
    private float aggroTimer;

    [Header("Attack")]
    private float attackTimer;

    void Start()
    {
        lineOfSight = GetComponent<LineOfSight>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = EnemyState.Patrolling;
        attackTimer = 0;
    }

    void Update()
    {
        //Tempos
        attackTimer -= Time.deltaTime;
        aggroTimer -= Time.deltaTime;

        //Transição de Estados
        UpdateStateTransitions();

        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }

    }

    void UpdateStateTransitions()
    {
        bool canSeePlayer = lineOfSight.CanSeePlayer();
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (canSeePlayer)
        {
            aggroTimer = stats.aggroDuration; // Reseta o timer de aggro
        }

        // Se o inimigo está aggro (viu o jogador recentemente)
        if (aggroTimer > 0)
        {
            if (distanceToPlayer <= stats.attackRange && canSeePlayer)
            {
                currentState = EnemyState.Attacking;
            }
            else
            {
                currentState = EnemyState.Chasing;
            }
        }
        else
        {
            currentState = EnemyState.Patrolling;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];
        if (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
        {
            MoveTowards(targetPoint.position, stats.patrolSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Para de se mover no ponto
            patrolWaitTimer += Time.deltaTime;
            if (patrolWaitTimer >= waitTimeAtPatrolPoint)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                patrolWaitTimer = 0f;
            }
        }
    }

    private void Chase()
    {
        MoveTowards(player.position, stats.chaseSpeed);
    }


    void Attack()
    {
        rb.linearVelocity = Vector2.zero;

        if (attackTimer <= 0f)
        {
            // Executa o ataque
            if (stats.attackType == AttackType.Melee)
            {
                PerformMeleeAttack();
            }
            else if (stats.attackType == AttackType.Ranged)
            {
                PerformRangedAttack();
            }

            // Reseta o cooldown
            attackTimer = stats.attackCooldown;
        }
    }

    void PerformMeleeAttack()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, stats.attackRange, playerLayer);

        foreach (Collider2D playerHit in hitPlayers)
        {
            Debug.Log("Hit " + playerHit.name);
            //PlayerHit Depois
        }
    }

    void PerformRangedAttack()
    {
        if (stats.projectilePrefab != null)
        {
            GameObject projectile = Instantiate(stats.projectilePrefab, transform.position, Quaternion.identity);
            Vector2 direction = (player.position - transform.position).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * stats.projectileSpeed;
            Destroy(projectile, 5f);
        }
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);
    }
}
