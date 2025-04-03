using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("Idle");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(!_enemyBase.HasTowerInRange)
            _stateMachine.ChangeState(_enemyBase.MoveState);
    }
}
