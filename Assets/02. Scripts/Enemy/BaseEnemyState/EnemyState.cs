using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class EnemyState
{
    protected EnemyStateMachine _stateMachine;
    protected Rigidbody2D _rigidbody;
    protected Enemy _enemyBase;
    private Bommer _bommer;

    protected bool _triggerCalled; //나중에 애니메이션 끝났다는거 알려주는 용도로 쓸거임
    private string _animBoolName; // 애니메이션 상태변환 할때 쓸거
    protected float _stateTimer;// 각상태마다 사용할 타이머임

    public virtual void AnimFinishTrigger() => _triggerCalled = true;

    public EnemyState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName)
    {
        _stateMachine = stateMachine;
        _rigidbody = rigidbody2D;
        _enemyBase = enemy;
        _animBoolName = animBoolName;

        _bommer = _enemyBase as Bommer;
    }
    protected virtual void SetAnimation(bool value)
    {
        if (!string.IsNullOrEmpty(_animBoolName))
            _enemyBase.Animator.SetBool(_animBoolName, value);
    }

    public virtual void Enter()
    {

        _stateTimer = 0;
        SetAnimation(true);
        _triggerCalled = false;
    }
    public virtual void Update()
    {
        _stateTimer -= Time.deltaTime;


        //목표 타겟이 없어졌으면
        if (_enemyBase.AttackTarget.activeSelf==false )
        {
            _enemyBase.RefreshTargetAndPath();
        }

    }
    public virtual void Exit()
    {
        SetAnimation(false);
    }

}
