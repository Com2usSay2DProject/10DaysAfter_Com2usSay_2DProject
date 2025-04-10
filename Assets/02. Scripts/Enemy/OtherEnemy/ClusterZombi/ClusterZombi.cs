using UnityEngine;

public class ClusterZombi : Enemy
{

    protected override void Awake()
    {
        base.Awake();

        MoveState = new ClusterMoveState(_stateMachine, _rigidbody2D, this, "Move");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
