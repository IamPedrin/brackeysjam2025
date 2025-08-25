using System;
using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private Vector3 _targetPos;
    private Vector3 _direction;


    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        _targetPos = GetRandomPointInCircle();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
        }

        _direction = (_targetPos - enemy.transform.position).normalized;
        enemy.MoveEnemy(_direction * enemy.RandomMovementSpeed);

        if((enemy.transform.position - _targetPos).sqrMagnitude < 0.01f)
        {
            _targetPos = GetRandomPointInCircle();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void ExitState()
    {
        base.ExitState();
        // Stop idle animation
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
        if (triggerType == Enemy.AnimationTriggerType.EnemyDamaged)
        {
            // Handle enemy damaged animation
        }
        else if (triggerType == Enemy.AnimationTriggerType.PlayFootStepSound)
        {
            // Handle footstep sound
        }
    }
    
    private Vector3 GetRandomPointInCircle()
    {
        return enemy.transform.position + (Vector3)UnityEngine.Random.insideUnitSphere * enemy.RandomMovementRange;
    }

}
