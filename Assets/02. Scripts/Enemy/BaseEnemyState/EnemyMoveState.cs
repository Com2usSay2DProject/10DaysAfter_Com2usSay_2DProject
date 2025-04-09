using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class EnemyMoveState : EnemyState
{

    public EnemyMoveState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();



    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (_enemyBase.IsDead == true)
            _stateMachine.ChangeState(_enemyBase.DeadState);


        if (_enemyBase.Path == null || _enemyBase.Path.Count == 0)
        {
            return;
        }


       //Vector2 avoidDir = AvoidLogic();

        Vector3 targetPoint = _enemyBase.Path.Peek();
        Vector2 toTarget = (targetPoint - _enemyBase.transform.position).normalized;

        Vector2 finalMove = toTarget.normalized;


        _rigidbody.linearVelocity = finalMove * _enemyBase.MoveSpeed;//* Time.fixedDeltaTime;


        _enemyBase.FaceDir = new Vector2(finalMove.x, finalMove.y);

        // 8방향 단순화
        Vector2 moveDir = new Vector2(
            Mathf.Round(toTarget.x),
            Mathf.Round(toTarget.y)
        );

        // 애니메이터에 넘기기
        _enemyBase.Animator.SetFloat("MoveX", toTarget.x);
        _enemyBase.Animator.SetFloat("MoveY", toTarget.y);



        if (Vector3.Distance(_enemyBase.transform.position, targetPoint) < 0.1f)
        {
            _enemyBase.Path.Dequeue();
        }


        if (_enemyBase.Path.Count == 0)
        {
            _rigidbody.linearVelocity = Vector2.zero;

        }

    }

    private Vector2 AvoidLogic()
    {
        Vector2 avoidDir = Vector2.zero;
        float avoidRadius = 0.7f; // 반응 거리 약간 증가

        Collider2D[] neighbors = Physics2D.OverlapCircleAll(
            _enemyBase.transform.position,
            avoidRadius,
            LayerMask.GetMask("Enemy")
        );

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == _enemyBase.gameObject) continue;

            Vector2 diff = (Vector2)_enemyBase.transform.position - (Vector2)neighbor.transform.position;
            float dist = diff.magnitude;

            if (dist > 0.01f)
            {
                // 가까우면 1, 멀면 0
                float strength = Mathf.Clamp01((avoidRadius - dist) / avoidRadius);
                avoidDir += diff.normalized * strength;
            }
        }

        // 회피 벡터 정규화 및 영향력 조절
        if (avoidDir != Vector2.zero)
        {
            avoidDir = avoidDir.normalized * 0.6f; // ← 회피 강도 조정 가능
        }

        return avoidDir;
    }
}
