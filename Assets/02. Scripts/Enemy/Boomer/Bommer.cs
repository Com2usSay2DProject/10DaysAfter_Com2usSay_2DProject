using UnityEngine;

public class Bommer : Enemy
{
    public bool isAttacked;
    protected override void Awake()
    {
        base.Awake();
        AttackState = new BommerAttackState(_stateMachine, _rigidbody2D,this, _spriteRenderer,"Attack");
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Update()
    {
        base.Update();
    }
}
