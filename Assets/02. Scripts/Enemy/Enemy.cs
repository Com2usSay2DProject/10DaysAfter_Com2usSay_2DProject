using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected static Dictionary<EEnemyType, EnemyData> _enemyDataDict;

    [Header("# Stat")]
    public EEnemyType EnemyType;
    protected EnemyData Data;

    protected EnemyStateMachine _stateMachine;
    protected Rigidbody2D _rigidbody2D;
    protected Animator _animator;
    public Animator Animator => _animator;
    protected SpriteRenderer _spriteRenderer;


    [SerializeField] private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;
    public bool HasTowerInRange = false;

    public Queue<Vector3> Path;



    #region Staties
    public EnemyIdleState IdleState;
    public EnemyMoveState MoveState;
    public EnemyAttackState AttackState;
    public EnemyDeathState DeathState;
    #endregion


    protected virtual void Awake()
    {
        //if (_enemyDataDict == null)
        //{
        //    GetData();
        //}
        //GetDataForThis();
        //_moveSpeed = Data.Speed;

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _stateMachine = new EnemyStateMachine();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); ;

        IdleState = new EnemyIdleState(_stateMachine, _rigidbody2D, this, "Idle");
        MoveState = new EnemyMoveState(_stateMachine, _rigidbody2D, this, "Move");
        AttackState = new EnemyAttackState(_stateMachine, _rigidbody2D, this, "Attack");
        DeathState = new EnemyDeathState(_stateMachine, _rigidbody2D, this, "Death");
    }

    private void GetData()
    {
        EnemyDataCollection collection =
            JsonDataManager.LoadFromFile<EnemyDataCollection>("Enemy/EnemyDataCollection");

        _enemyDataDict = new Dictionary<EEnemyType, EnemyData>();

        foreach(EnemyData data in collection.Datas)
        {
            data.TypeString = data.EnemyType.ToString();
            _enemyDataDict[data.EnemyType] = data;
        }

        Debug.Log("적 데이터 로드 완료");
    }

    private void GetDataForThis()
    {
        if(_enemyDataDict.TryGetValue(EnemyType, out EnemyData data))
        {
            Data = new EnemyData();
            Data = data;
        }
        else
        {
            Debug.LogError($"적 데이터 없음{EnemyType}");
        }
    }

    private void DeadEnemy() { _stateMachine.ChangeState(DeathState); }

    protected virtual void Start()
    {
        if(PhaseManager.Instance)
            PhaseManager.Instance.OnNightEnd += DeadEnemy;

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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("MainTower"))
        {
            _stateMachine.ChangeState(AttackState);
            HasTowerInRange = true;

        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        HasTowerInRange = false;
    }

}
