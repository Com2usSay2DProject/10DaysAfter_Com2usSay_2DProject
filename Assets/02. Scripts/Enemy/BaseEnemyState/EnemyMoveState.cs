using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveState : EnemyState
{

    public EnemyMoveState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
 
        //List<Vector3> pathList = Pathfinding.FindPath(startPos, targetPos);
        //if (pathList != null && pathList.Count > 0)
        //{
        //    _path = new Queue<Vector3>(pathList);
        //}
        //else
        //{
        //    Debug.LogError("경로를 찾을 수 없습니다.");
        //}

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (_enemyBase.Path == null || _enemyBase.Path.Count == 0)
        {
            return;
        }


        Vector3 targetPoint = _enemyBase.Path.Peek();
        Vector3 direction = (targetPoint - _enemyBase.transform.position).normalized;
        _rigidbody.linearVelocity = direction * _enemyBase.MoveSpeed;

        // 8방향 단순화
        Vector2 moveDir = new Vector2(
            Mathf.Round(direction.x),
            Mathf.Round(direction.y)
        );

        // 애니메이터에 넘기기
        _enemyBase.Animator.SetFloat("MoveX", moveDir.x);
        _enemyBase.Animator.SetFloat("MoveY", moveDir.y);



        if (Vector3.Distance(_enemyBase.transform.position, targetPoint) < 0.1f)
        {
            _enemyBase.Path.Dequeue();
        }


        if (_enemyBase.Path.Count == 0)
        {
            _rigidbody.linearVelocity = Vector2.zero;

        }

    }
}
