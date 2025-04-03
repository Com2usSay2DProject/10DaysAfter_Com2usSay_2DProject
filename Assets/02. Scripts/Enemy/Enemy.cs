using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private EnemyStateMachine _stateMachine;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    public Animator Animator => _animator;
    private SpriteRenderer _spriteRenderer;


    [SerializeField] private float _moveSpeed = 10;
    public float MoveSpeed => _moveSpeed;
    public bool HasTowerInRange = false;

    public Queue<Vector3> Path;



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
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); ;

        IdleState = new EnemyIdleState(_stateMachine, _rigidbody2D, this, "Idle");
        MoveState = new EnemyMoveState(_stateMachine, _rigidbody2D, this, "Move");
        AttackState = new EnemyAttackState(_stateMachine, _rigidbody2D, this, "Attack");
        DeathState = new EnemyDeathState(_stateMachine, _rigidbody2D, this, "Death");
    }

    void Start()
    {
        if (_stateMachine != null)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            _stateMachine.ChangeState(AttackState);
            HasTowerInRange = true;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        HasTowerInRange = false;
    }

    public void FlipSprite(Vector3 direction)
    {

        _spriteRenderer.flipX = direction.x < 0;
    }
}
