using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyState
{
 

    public EnemyAttackState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(_stateTimer<0)
        {
            _stateTimer = _enemyBase.AttackRate;
        }
        if (!_enemyBase.HasTowerInRange && !_enemyBase.IsDead)
        {
            _stateMachine.ChangeState(_enemyBase.MoveState);

            List<Vector3> pathList = Pathfinding.FindPath(_enemyBase.transform.position, _enemyBase.TargetSelector.FindTarget(TargetType.MainTower).position);
            if (pathList != null && pathList.Count > 0)
            {
                _enemyBase.Path = new Queue<Vector3>(pathList);
            }
            else
            {
                Debug.LogError("경로를 찾을 수 없습니다.");
            }

            _stateMachine.ChangeState(_enemyBase.MoveState);
        }

    }
}
