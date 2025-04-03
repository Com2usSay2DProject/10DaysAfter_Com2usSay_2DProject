using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private float _attackRate;

    public EnemyAttackState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

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
            _stateTimer = _attackRate;

            Debug.Log("Attack");
        }
        
        if(_enemyBase.HasTowerInRange == false)
        {
            _stateMachine.ChangeState(_enemyBase.IdleState);
        }

    }
}
