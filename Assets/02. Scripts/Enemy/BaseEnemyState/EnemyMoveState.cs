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

        // 시작 위치와 목표 위치 설정
        Vector3 startPos = _enemyBase.transform.position;
        Vector3 targetPos = Vector3.zero; // 중앙 기지 위치 (필요에 따라 수정)

        // 커스텀 A* 알고리즘을 통해 경로를 계산
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
            // 경로가 없거나 모두 완료된 경우 처리 (예: 대기, 상태 전환 등)
            return;
        }

        // 다음 경유지 좌표 확인
        Vector3 targetPoint = _path.Peek();
        // 현재 위치에서 목표까지의 방향 계산
        Vector3 direction = (targetPoint - _enemyBase.transform.position).normalized;
        // Rigidbody2D를 이용해 이동 처리
        _rigidbody.linearVelocity = direction * _enemyBase._moveSpeed;

        // 목표 지점에 충분히 도달했으면 해당 좌표 제거
        if (Vector3.Distance(_enemyBase.transform.position, targetPoint) < 0.1f)
        {
            _path.Dequeue();
        }

        // 모든 경로를 완료한 경우, 이동을 멈추고 다음 상태(예: 공격)로 전환 처리
        if (_path.Count == 0)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            // 예: _stateMachine.ChangeState(_enemyBase.AttackState);
        }
    }
}
