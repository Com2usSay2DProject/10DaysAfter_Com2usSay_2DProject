using UnityEngine;

public class ClusterMoveState : EnemyMoveState
{
    public ClusterMoveState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _stateTimer = MoveSoundTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        if (_enemyBase.IsDead == true)
            _stateMachine.ChangeState(_enemyBase.DeadState);

        if (_stateTimer < 0)
        {
            _stateTimer = MoveSoundTime;
            if (EnemySoundManager.Instance != null)
            {
                EnemySoundManager.Instance.Play3DSoundWithLimit(_enemyBase.gameObject.transform.position, EnemySoundType.EnemyMove);
            }
        }
        _avoidCheckTimer -= Time.deltaTime;
        if (_avoidCheckTimer <= 0f)
        {
            _cachedAvoidDir = AvoidLogic();
            _avoidCheckTimer = 0.3f; // 0.3초마다 한 번만 회피 계산
        }
        Vector2 avoidDir = _cachedAvoidDir;



        Vector2 toTarget = (Vector3.zero - _enemyBase.transform.position).normalized;

        Vector2 finalMove = toTarget.normalized + avoidDir;


        _rigidbody.linearVelocity = finalMove * _enemyBase.MoveSpeed;//* Time.fixedDeltaTime;

        Vector2 moveDir = new Vector2( Mathf.Round(toTarget.x), Mathf.Round(toTarget.y));

        // 애니메이터에 넘기기
        _enemyBase.Animator.SetFloat("MoveX", toTarget.x);
        _enemyBase.Animator.SetFloat("MoveY", toTarget.y);

        if (!_enemyBase.IsDead)
            UpdateTargetAndPath();
        //if (!_enemyBase.IsDead)
        //    UpdateTargetAndPath();



    }
}
