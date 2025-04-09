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


    public virtual void Enter()
    {
        Debug.Log(_animBoolName);

        _stateTimer = 0;
        _enemyBase.Animator.SetBool(_animBoolName, true);
        _triggerCalled = false;
    }
    public virtual void Update()
    {
        _stateTimer -= Time.deltaTime;

        if(!_enemyBase.IsDead)
            UpdateTargetAndPath();
    }

    private void UpdateTargetAndPath()
    {
        Debug.Log("update");

        if(_bommer)
        {
            if (_bommer.isAttacked) return;
        }

        if (_enemyBase.AttackTerget == null || !_enemyBase.AttackTerget.activeSelf)
        {
            _enemyBase.RefreshTargetAndPath();

            if (_enemyBase.AttackTerget != null)
            {
                _stateMachine.ChangeState(_enemyBase.MoveState);
            }
        }
    }

    public virtual void Exit()
    {
        _enemyBase.Animator.SetBool(_animBoolName, false);
    }

}
