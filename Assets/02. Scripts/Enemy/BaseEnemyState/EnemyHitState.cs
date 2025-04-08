using System.Collections;
using UnityEngine;

public class EnemyHitState : EnemyState
{
    public EnemyHitState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if(_enemyBase.HitBloodPrefab)
        {
            GameObject.Instantiate(_enemyBase.HitBloodPrefab, _enemyBase.gameObject.transform);
        }
        _rigidbody.linearVelocity = Vector2.zero;
        //_stateTimer = 0.5f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (_triggerCalled)
        {
                _stateMachine.ChangeState(_enemyBase.IdleState);
            _stateTimer -= Time.deltaTime;

            if(_stateTimer<=0)
            {
            }
        }
    }
}
