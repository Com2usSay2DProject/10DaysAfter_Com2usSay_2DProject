using UnityEngine;

public class EnemyState
{
    protected EnemyStateMachine _stateMachine;
    protected Rigidbody2D _rigidbody;
    protected Enemy _enemyBase;

    protected bool _triggerCalled; //나중에 애니메이션 끝났다는거 알려주는 용도로 쓸거임
    private string _animBoolName; // 애니메이션 상태변환 할때 쓸거

   public EnemyState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName)
    {
        _stateMachine = stateMachine;
        _rigidbody = rigidbody2D;
        _enemyBase = enemy;
        _animBoolName = animBoolName;
    }

    protected float _stateTimer;// 각상태마다 사용할 타이머임

    public virtual void Enter()
    {

    }
    public virtual void Update()
    {

    }
    public virtual void Exit()
    {

    }

}
