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
    public enum InitialBehavior {Idle, Patrol}

    [Header("Configurações Gerais")]
    public EnemyStats stats;
    public Transform[] patrolPoints;
    public LayerMask playerLayer;

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
    private bool wasPatrolling;
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

        // switch (currentState)
        // {
        //     case EnemyState.Patrolling:
        //         Patrol();
        //         break;
        //     case EnemyState.Chasing:
        //         Chase();
        //         break;
        //     case EnemyState.Attacking:
        //         Attack();
        //         break;
        // }
    }

    void UpdateStateTransitions()
    {
        bool canSeePlayer = lineOfSight.CanSeePlayer();
        
        // --- LÓGICA DE AGGRO (MAIOR PRIORIDADE) ---
        if (canSeePlayer)
        {
            aggroTimer = stats.aggroDuration;
            returningFromChase = true; // Marca que ele esteve em perseguição
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
            return; // Sai da função para não executar outras transições
        }

        // --- LÓGICA APÓS PERDER O AGGRO ---
        if (returningFromChase)
        {
            currentState = EnemyState.ReturningToStart;
            returningFromChase = false; // Reseta a flag
        }

        // --- LÓGICA DOS ESTADOS NORMAIS ---
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
                rb.linearVelocity = Vector2.zero; // Ação: Ficar parado
                
                break;
            case EnemyState.Patrolling:
                MoveTowards(patrolPoints[currentPatrolIndex].position, stats.patrolSpeed);
                
                break;
            case EnemyState.ReturningToStart:
                MoveTowards(startPosition, stats.chaseSpeed); // Volta rápido
                
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
