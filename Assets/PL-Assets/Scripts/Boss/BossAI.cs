using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BossHealth))] // Agora requer o novo script de vida
public class BossAI : MonoBehaviour
{
    public enum BossState { Idle, CardAttack_Telegraph, CardAttack_Active, CoinAttack }

    [Header("Configuração")]
    public BossStats bossStats;
    public Transform player;
    public Collider2D bossRoomBounds;
    public Transform coinFirePoint;

    private BossState currentState;
    private float stateTimer;
    private List<GameObject> activeIndicators = new List<GameObject>();
    private List<Vector2> cardAttackPositions = new List<Vector2>();
    private BossState nextStateAfterIdle = BossState.CardAttack_Telegraph;

    void Start()
    {
        // A validação de vida agora é responsabilidade do BossHealth
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetState(BossState.Idle);
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer <= 0) Invoke(nameof(TransitionFromIdle), 0);
                break;
            case BossState.CardAttack_Telegraph:
                if (stateTimer <= 0) SetState(BossState.CardAttack_Active);
                break;
            case BossState.CardAttack_Active:
                if (stateTimer <= 0) SetState(BossState.Idle, true);
                break;
            case BossState.CoinAttack:
                // Lógica de transição controlada pela Corrotina
                break;
        }
    }

    void SetState(BossState newState, bool prepareNextAttack = false)
    {
        currentState = newState;

        switch (currentState)
        {
            case BossState.Idle:
                stateTimer = bossStats.idleDuration;
                if (prepareNextAttack)
                {
                    nextStateAfterIdle = (nextStateAfterIdle == BossState.CardAttack_Telegraph) ? BossState.CoinAttack : BossState.CardAttack_Telegraph;
                }
                break;
            case BossState.CardAttack_Telegraph:
                stateTimer = bossStats.cardAttackTelegraphDuration;
                SpawnCardWarnings();
                break;
            case BossState.CardAttack_Active:
                stateTimer = bossStats.cardAttackActiveDuration;
                ActivateCardAttacks();
                break;
            case BossState.CoinAttack:
                StartCoroutine(CoinAttackSequence());
                break;
        }
    }

    void TransitionFromIdle()
    {
        SetState(nextStateAfterIdle);
    }

    void SpawnCardWarnings()
    {
        cardAttackPositions.Clear();
        Bounds bounds = bossRoomBounds.bounds;

        for (int i = 0; i < bossStats.numberOfCardAttacks; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y));
            cardAttackPositions.Add(randomPos);
            GameObject indicator = Instantiate(bossStats.warningIndicatorPrefab, randomPos, Quaternion.identity);
            activeIndicators.Add(indicator);
        }
    }

    void ActivateCardAttacks()
    {
        foreach (GameObject indicator in activeIndicators) Destroy(indicator);
        activeIndicators.Clear();

        foreach (Vector2 pos in cardAttackPositions)
        {
            GameObject cardAttack = Instantiate(bossStats.cardAttackPrefab, pos, Quaternion.identity);
            cardAttack.GetComponent<CardAttack>().Setup(bossStats.cardAttackDamage);
        }
    }

    System.Collections.IEnumerator CoinAttackSequence()
    {
        for (int i = 0; i < bossStats.numberOfCoinsToThrow; i++)
        {
            GameObject coin = Instantiate(bossStats.coinProjectilePrefab, coinFirePoint.position, coinFirePoint.rotation);
            coin.GetComponent<EnemyProjectile>().Setup(bossStats.coinAttackDamage, bossStats.coinProjectileSpeed);
            yield return new WaitForSeconds(bossStats.timeBetweenCoins);
        }
        SetState(BossState.Idle, true);
    }
}