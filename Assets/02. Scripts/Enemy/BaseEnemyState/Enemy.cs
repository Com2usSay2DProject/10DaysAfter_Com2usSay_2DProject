using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    protected static Dictionary<EEnemyType, EnemyData> _enemyDataDict;

    [Header("# Stat")]
    [SerializeField] protected EnemyData Data;
    public EEnemyType EnemyType;
    public float Hp;

    [Header("# Prefab")]
    public GameObject ProjectilePrefab;
    public GameObject HitBloodPrefab;


    protected EnemyStateMachine _stateMachine;
    protected Rigidbody2D _rigidbody2D;
    protected CircleCollider2D _collider2D;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    public EnemyTargetSelector TargetSelector;

    //게터
    public Animator Animator => _animator;
    public float MoveSpeed => Data.Speed;
    public float AttackRange => Data.Range;
    public float AttackRate => Data.AtkSpeed;
    public float Damage => Data.Damage;
    public ETargetType TargetType => Data.TargetType;


    public bool HasTowerInRange = false;
    public bool IsDead = false;


    public Queue<Vector3> Path; // 현재 이동경로
    public Vector2 FaceDir; //현재 보는 방향
    public GameObject AttackTerget = null; //현재 목표인 오브젝트

    #region Staties
    public EnemyIdleState IdleState;
    public EnemyMoveState MoveState;
    public EnemyAttackState AttackState;
    public EnemyHitState HitState;
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

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _stateMachine = new EnemyStateMachine();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); ;
        _collider2D = GetComponent<CircleCollider2D>();


        IdleState = new EnemyIdleState(_stateMachine, _rigidbody2D, this, "Idle");
        MoveState = new EnemyMoveState(_stateMachine, _rigidbody2D, this, "Move");
        AttackState = new EnemyAttackState(_stateMachine, _rigidbody2D, this, "Attack");
        HitState = new EnemyHitState(_stateMachine, _rigidbody2D, this, "Hit");
        DeadState = new EnemyDeadState(_stateMachine, _rigidbody2D, this, "Dead", _spriteRenderer,_collider2D);
    }

    protected virtual void OnEnable()
    {

        if (_stateMachine.IsInited)
            _stateMachine.ChangeState(IdleState);

        int bonusDay = Mathf.Max(0, PhaseManager.Instance.CurrentDay - 1);
        float baseGrowth = 1.18f;
        float bonusMultiplier = Mathf.Pow(baseGrowth, bonusDay); 
        Hp = (int)(Data.MaxHp * bonusMultiplier);

        IsDead = false;
        _collider2D.enabled = true;
        _spriteRenderer.color = new Color(1, 1, 1, 1);
        HasTowerInRange = false;

    }

    private void GetData()
    {
        EnemyDataCollection collection;

#if UNITY_EDITOR
        // 에디터에서는 파일 직접 로딩
        collection = JsonDataManager.LoadFromFile<EnemyDataCollection>("Enemy/EnemyDataCollection");
#else
    // 빌드 후에는 Resources.Load 사용
    TextAsset jsonText = Resources.Load<TextAsset>("Json/Enemy/EnemyDataCollection");
    if (jsonText == null)
    {
        Debug.LogError("적 데이터 파일을 찾을 수 없습니다 (빌드 환경)");
        return;
    }
    collection = JsonDataManager.FromJson<EnemyDataCollection>(jsonText.text);
#endif

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
            Data = data;
        }
        else
        {
            Debug.LogError($"적 데이터 없음{EnemyType}");
        }
    }

    private void OnDayBegin() 
    {
    }
    private void OnNightBegin()
    {
    }

    protected virtual void Start()
    {
        if (PhaseManager.Instance != null)
        {
            PhaseManager.Instance.OnDayBegin += OnDayBegin;
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

        _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);

#if UNITY_EDITOR
        //디버그용
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TakeDamage(0);
        }
#endif
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsDead) return;

        Hp -= damage;
        //일단 공격상태로 진입했으면 히트 상태로 안감 
        if(_stateMachine.currentState != AttackState)
            _stateMachine.ChangeState(HitState);
        if (Hp <= 0)
        {
            IsDead = true;
            _stateMachine.ChangeState(DeadState);
        }
    }
    public void CanAttack()
    {
        if (IsDead) return;

        _stateMachine.ChangeState(AttackState);
        HasTowerInRange = true;
    }

    public void RefreshTargetAndPath()
    {
        // 지정된 타입 타겟 재탐색
        AttackTerget = TargetSelector.FindTarget(TargetType);

        // 없으면 메인 타워를 타겟으로
        if (AttackTerget == null)
            AttackTerget = TargetSelector.FindTarget(ETargetType.MainTower);

        if (AttackTerget != null)
        {
            
            List<Vector3> path = Pathfinding.FindPath(transform.position, AttackTerget.transform.position);
            if (path != null && path.Count > 0)
                Path = new Queue<Vector3>(path);
        }
        else
        {
            Debug.Log("타겟이 존재하지 않습니다.");
        }
    }


}
