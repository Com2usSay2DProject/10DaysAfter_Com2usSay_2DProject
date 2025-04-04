using UnityEngine;

public class EnemyDeathState : EnemyState
{
    public EnemyDeathState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        PoolManager.Instance.ReturnObject(_enemyBase.gameObject, EObjectType.Enemy);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
