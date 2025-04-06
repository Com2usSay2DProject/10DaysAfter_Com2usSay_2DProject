using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected static Dictionary<EEnemyType, EnemyData> _enemyDataDict;

    [Header("# Stat")]
    [SerializeField] protected EnemyData Data;
    public EEnemyType EnemyType;
    public float Hp;



    protected EnemyStateMachine _stateMachine;
    protected Rigidbody2D _rigidbody2D;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    public EnemyTargetSelector TargetSelector;
    
    //게터
    public Animator Animator => _animator;
    public float MoveSpeed => Data.Speed;
    public float AttackRange => Data.Range;
    public float AttackRate => Data.AtkSpeed;
    public float Damage => Data.Damage;



    public bool HasTowerInRange = false;
    public bool IsDead = false;


    public Queue<Vector3> Path; // 현재 이동경로
    public Vector2 FaceDir; //현재 보는 방향

    #region Staties
    public EnemyIdleState IdleState;
    public EnemyMoveState MoveState;
    public EnemyAttackState AttackState;
    public EnemyDeadState DeadState;
    #endregion


    public void AnimTrigger() => _stateMachine.currentState.AnimFinishTrigger();

    protected virtual void Awake()
    {
        if (_enemyDataDict == null)
        {
            GetData();
        }
        GetDataForThis();

        Hp = Data.MaxHp;

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _stateMachine = new EnemyStateMachine();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); ;

        IdleState = new EnemyIdleState(_stateMachine, _rigidbody2D, this, "Idle");
        MoveState = new EnemyMoveState(_stateMachine, _rigidbody2D, this, "Move");
        AttackState = new EnemyAttackState(_stateMachine, _rigidbody2D, this, "Attack");
        DeadState = new EnemyDeadState(_stateMachine, _rigidbody2D, this, "Dead", _spriteRenderer);
    }

    private void GetData()
    {
        EnemyDataCollection collection =
            JsonDataManager.LoadFromFile<EnemyDataCollection>("Enemy/EnemyDataCollection");

        _enemyDataDict = new Dictionary<EEnemyType, EnemyData>();

        foreach (EnemyData data in collection.Datas)
        {
            data.TypeString = data.EnemyType.ToString();
            _enemyDataDict[data.EnemyType] = data;
        }

        Debug.Log("적 데이터 로드 완료");
    }

    private void GetDataForThis()
    {
        if (_enemyDataDict.TryGetValue(EnemyType, out EnemyData data))
        {
            Data = new EnemyData();
            Data = data;
        }
        else
        {
            Debug.LogError($"적 데이터 없음{EnemyType}");
        }
    }

    private void DeadEnemy() { _stateMachine.ChangeState(DeadState); }
    private void OnNightBegin()
    {
        _stateMachine.ChangeState(IdleState);
        _spriteRenderer.color = new Color(1, 1, 1, 1);
        IsDead = false;
    }

    protected virtual void Start()
    {
        if (PhaseManager.Instance != null)
        {
            PhaseManager.Instance.OnDayBegin += DeadEnemy;
            PhaseManager.Instance.OnNightBegin += OnNightBegin;
        }

        if (_stateMachine != null)
        {
            _stateMachine.InitStateMachine(IdleState, this);
        }


    }

    protected virtual void Update()
    {
        _stateMachine.Update();
    }

    protected virtual void TakeDamage(float damage)
    {

    }
    public void CanAttack()
    {
        if (IsDead) return;

        _stateMachine.ChangeState(AttackState);
        HasTowerInRange = true;
    }

}
