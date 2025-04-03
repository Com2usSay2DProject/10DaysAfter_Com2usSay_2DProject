using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyState
{
    private Queue<Vector3> _path;

    public EnemyMoveState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();


        Vector3 startPos = _enemyBase.transform.position;
        Vector3 targetPos = _enemyBase.Target.position;

 
        List<Vector3> pathList = Pathfinding.FindPath(startPos, targetPos);
        if (pathList != null && pathList.Count > 0)
        {
            _path = new Queue<Vector3>(pathList);
        }
        else
        {
            Debug.LogError("경로를 찾을 수 없습니다.");
        }

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (_path == null || _path.Count == 0)
        {
            return;
        }


        Vector3 targetPoint = _path.Peek();

        Vector3 direction = (targetPoint - _enemyBase.transform.position).normalized;

        _rigidbody.linearVelocity = direction * _enemyBase._moveSpeed;


        if (Vector3.Distance(_enemyBase.transform.position, targetPoint) < 0.1f)
        {
            _path.Dequeue();
        }


        if (_path.Count == 0)
        {
            _rigidbody.linearVelocity = Vector2.zero;

        }
    }
}
