using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrolling,
    Chasing,
    Attacking,
    ReturningToStart
}

public class EnemyAI : MonoBehaviour
{
    public enum InitialBehavior { Idle, Patrol }

    [Header("Configurações Gerais")]
    public EnemyStats stats;
    public Transform[] patrolPoints;
    public LayerMask playerLayer;
    public Transform projectileAttackPoint;

    [Header("Comportamento da IA")]
    public InitialBehavior initialBehavior = InitialBehavior.Patrol;
    public float waitTimeAtPatrolPoint = 2f;
    private EnemyState currentState;

    //Componentes e referencias
    private LineOfSight lineOfSight;
    private Transform player;
    private Rigidbody2D rb;

    //Controle de estado
    private Vector2 startPosition;
    private bool isFacingRight = true;
    private bool returningFromChase = false;


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
        startPosition = transform.position;

        if (initialBehavior == InitialBehavior.Patrol && patrolPoints.Length > 0)
        {
            currentState = EnemyState.Patrolling;
        }
        else
        {
            currentState = EnemyState.Idle;
        }

        attackTimer = 0;
    }

    void Update()
    {
        //Tempos
        attackTimer -= Time.deltaTime;
        aggroTimer -= Time.deltaTime;

        //Transição de Estados
        UpdateStateTransitions();
        ExecuteCurrentStateAction();
        UpdateSpriteDirection();
    }

    void UpdateStateTransitions()
    {
        bool canSeePlayer = lineOfSight.CanSeePlayer();


        if (canSeePlayer)
        {
            aggroTimer = stats.aggroDuration;
            returningFromChase = true;
        }

        if (aggroTimer > 0)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= stats.attackRange && canSeePlayer)
            {
                currentState = EnemyState.Attacking;
            }
            else
            {
                currentState = EnemyState.Chasing;
            }
            return;
        }


        if (returningFromChase)
        {
            currentState = EnemyState.ReturningToStart;
            returningFromChase = false;
        }


        switch (currentState)
        {
            case EnemyState.ReturningToStart:
                if (Vector2.Distance(transform.position, startPosition) < 0.1f)
                {
                    currentState = EnemyState.Idle; // Chegou ao início, fica ocioso
                }
                break;
            case EnemyState.Patrolling:
                if (patrolPoints.Length > 0 && Vector2.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < 0.1f)
                {
                    currentState = EnemyState.Idle; // Chegou a um ponto, fica ocioso
                }
                break;
            case EnemyState.Idle:
                // Se ele estava patrulhando antes, espera e volta a patrulhar
                if (initialBehavior == InitialBehavior.Patrol && patrolPoints.Length > 0)
                {
                    patrolWaitTimer += Time.deltaTime;
                    if (patrolWaitTimer >= waitTimeAtPatrolPoint)
                    {
                        patrolWaitTimer = 0f;
                        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                        currentState = EnemyState.Patrolling;
                    }
                }
                // Se ele é um guarda estático, ele simplesmente fica em Idle.
                break;
        }
    }

    void ExecuteCurrentStateAction()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                rb.linearVelocity = Vector2.zero;

                break;
            case EnemyState.Patrolling:
                MoveTowards(patrolPoints[currentPatrolIndex].position, stats.patrolSpeed);

                break;
            case EnemyState.ReturningToStart:
                MoveTowards(startPosition, stats.chaseSpeed);

                break;
            case EnemyState.Chasing:
                Chase();

                break;
            case EnemyState.Attacking:
                Attack();

                break;
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
            rb.linearVelocity = Vector2.zero;
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

            PlayerHealth playerHealth = playerHit.GetComponent<PlayerHealth>();


            if (playerHealth != null)
            {
                Debug.Log("Inimigo Melee acertou o jogador com " + stats.attackDamage + " de dano.");
                playerHealth.TakeDamage(stats.attackDamage);
            }
        }
    }

    void PerformRangedAttack()
    {
        if (stats.projectilePrefab != null)
        {
            Vector2 spawnPosition = transform.position;
            Vector2 direction = (player.position - (Vector3)spawnPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            GameObject projectileGO = Instantiate(stats.projectilePrefab, spawnPosition, rotation);

            EnemyProjectile projectile = projectileGO.GetComponent<EnemyProjectile>();

            if (projectile != null)
            {
                projectile.Setup(stats.attackDamage, stats.projectileSpeed);
            }
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

    void UpdateSpriteDirection()
    {

        if (rb.linearVelocity.x < -0.1f && isFacingRight)
        {
            Flip();
        }

        else if (rb.linearVelocity.x > 0.1f && !isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 ls = transform.localScale;
        ls.x *= -1f;
        transform.localScale = ls;
    }
}
