using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private EnemyStateMachine _stateMachine;
    private Rigidbody2D _rigidbody2D;


    private float _moveSpeed;


    #region Staties
    public EnemyIdleState IdleState;
    public EnemyMoveState MoveState;
    public EnemyAttackState AttackState;
    public EnemyDeathState DeathState;
    #endregion


    protected virtual void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _stateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(_stateMachine, _rigidbody2D, this, "Idle");
        MoveState = new EnemyMoveState(_stateMachine, _rigidbody2D, this, "Move");
        AttackState = new EnemyAttackState(_stateMachine, _rigidbody2D, this, "Attack");
        DeathState = new EnemyDeathState(_stateMachine, _rigidbody2D, this, "Death");
    }

    void Start()
    {
        if(_stateMachine != null)
        {
            _stateMachine.InitStateMachine(IdleState, this);
        }
    }

    void Update()
    {
        _stateMachine.Update();
    }

    protected virtual void TakeDamage(float damage)
    {

    }


}
