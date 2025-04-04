using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;

public class TowerRoot : MonoBehaviour
{
    private static Dictionary<ETowerType, TowerData> _towerDataDict; // 모든 타워가 공유할 데이터

    [Header("# Stats")]
    public ETowerType TowerType; // 타워의 타입 -> 프리팹에서 설정해두면 데이터 찾아옴
    protected TowerData Data; // 해당 타워의 데이터
    protected float _maxHp;
    protected float _hp;
    protected float _damage;
    protected float _atkSpeed;
    protected float _range;

    [SerializeField]
    private GameObject UpgradeUI;

    [Header("# State")]
    private bool _isEnemyDetected = false;
    [SerializeField]
    private GameObject _targetEnemy;

    private void Awake()
    {
        GetData();
        GetDataForThis();
    }

    private void Start()
    {
        ObserveTargetEnemy();
        ResourceManager.Instance.OnPopulationChange += MultiplyData;
    }

    private void ObserveTargetEnemy()
    {
        this.ObserveEveryValueChanged(_ => _targetEnemy)
            .Where(target => target == null)
            .Subscribe(_ =>
            {
                Debug.Log("타워 : 타겟 사라짐");
                _isEnemyDetected = false;
            }).AddTo(this);
    }

    private void Update()
    {
        if(!_isEnemyDetected || !_targetEnemy)
        {
            _targetEnemy = DetectEnemy();
            _isEnemyDetected = _targetEnemy != null;
        }
        else
        {
            Attack();
        }
    }
    
    #region Data
    private void GetData()
    {
        // JSON 데이터가 아직 로드되지 않았다면 불러오기
        if (_towerDataDict == null)
        {
            //LoadTowerData();
            TowerDataCollection collection =
                JsonDataManager.LoadFromFile<TowerDataCollection>("Tower/TowerDataCollection");

            _towerDataDict = new Dictionary<ETowerType, TowerData>();

            foreach (TowerData d in collection.Datas)
            {
                d.TypeString = d.TowerType.ToString();
                _towerDataDict[d.TowerType] = d;
            }

            Debug.Log("타워 데이터 로드 완료");
        }
    }

    private void GetDataForThis()
    {
        // 내 타워 타입에 맞는 데이터 찾기
        if (_towerDataDict.TryGetValue(TowerType, out TowerData data))
        {
            Data = new TowerData();
            Data = data;

            _maxHp = Data.MaxHp;
            _hp = _maxHp;
            _damage = Data.Damage;
            _atkSpeed = Data.AtkSpeed;
            _range = Data.Range;
        }
        else
        {
            Debug.LogError($"타워 데이터 없음: {TowerType}");
        }
    }

    private void MultiplyData()
    {
        _maxHp = Data.GetModifiedStat(Data.MaxHp, ResourceManager.Instance.GetResourceAmount(ResourceType.Population));
        _damage = Data.GetModifiedStat(Data.Damage, ResourceManager.Instance.GetResourceAmount(ResourceType.Population));
        _atkSpeed = Data.GetModifiedStat(Data.AtkSpeed, ResourceManager.Instance.GetResourceAmount(ResourceType.Population));
        _range = Data.GetModifiedStat(Data.Range, ResourceManager.Instance.GetResourceAmount(ResourceType.Population));
    }
    #endregion

    #region 행동
    private GameObject DetectEnemy()
    {
        GameObject target = null;
        float minDistance = float.MaxValue;

        GameObject[] Enemys = Physics2D
            .OverlapCircleAll(transform.position, _range, 1 << LayerMask.NameToLayer("Enemy"))
            .Select(c => c.gameObject).ToArray();

        foreach(GameObject enemy in Enemys)
        {
            float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if(currentDistance <= _range && currentDistance <= minDistance)
            {
                minDistance = currentDistance;
                target = enemy;
            }
        }
        if(target != null)
        {
            _isEnemyDetected = true;
        }

        return target;
    }

    protected virtual void Attack()
    {
        //공격 로직 이거 상속받아서 구현
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;

        //TODO : 피격 이펙트

        if(_hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //TODO : 폭발 이펙트
        TowerPoolManager.Instance.ReturnObject(gameObject, TowerType);
    }
    #endregion

    public void TowerClick()
    {
        // 현재 클릭된 타워 제외 나머지 타워 클릭 UI 비활성화
        TileClickManager.Instance.TowerClick += () => UpgradeUI.SetActive(false);
        UpgradeUI.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}