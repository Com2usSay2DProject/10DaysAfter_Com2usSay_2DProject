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
            _stateTimer = _attackRate;
        }
        

    }
}
