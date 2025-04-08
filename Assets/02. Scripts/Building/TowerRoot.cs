using System.Collections.Generic;
using UnityEngine;

public class TowerRoot : MonoBehaviour // 공통 속성 및 건설 관련 로직만 여기에
{
    private static Dictionary<ETowerType, TowerData> _towerDataDict; // 모든 타워가 공유할 데이터

    [Header("# Stats")] 
    public ETowerType TowerType { get; private set; } // 타워의 타입 -> 프리팹에서 설정해두면 데이터 찾아옴
    protected TowerData Data; // 해당 타워의 데이터
    protected float _maxHp;
    protected float _hp;
    protected float _damage;
    protected float _atkSpeed;
    protected float _range;

    [Header("# Cost")]
    public Dictionary<ResourceType, int> CostDataDict { get; private set; }

    [Header("# State")]
    public bool IsBuilt { get; set; }

    [Header ("# Buildable")]
    public bool CanBuild { get; private set; }
    private HashSet<Collider2D> _overlappingColliders = new HashSet<Collider2D>(); // 건설 가능 판정용

    [Header ("# Components")]
    protected SpriteRenderer _spriteRenderer;
    protected Rigidbody2D _rigid;
    private Color _tempColor = new Color(255, 255, 255, 0.5f);
    private Color _errorColor = new Color(255, 0, 0, 0.5f);

    #region Common
    protected virtual void Awake() 
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigid = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable() 
    {
        GetData();
        GetDataForThis();
        GetCostData();
        IsBuilt = false;
        CanBuild = true;
        _spriteRenderer.color = _tempColor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsBuilt) return;

        if (collision.CompareTag("Tower"))
        {
            _overlappingColliders.Add(collision);
            CanBuild = false;
            _spriteRenderer.color = _errorColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsBuilt) return;

        if (collision.CompareTag("Tower"))
        {
            _overlappingColliders.Remove(collision);

            // 아무것도 안 겹칠 때만 가능하게
            if (_overlappingColliders.Count == 0)
            {
                CanBuild = true;
                _spriteRenderer.color = _tempColor;
            }
        }
    }

    public virtual void SetPosition()
    {
        _spriteRenderer.color = Color.white;
        IsBuilt = true;
        _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;

        //TODO : 피격 이펙트

        if (_hp <= 0)
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

    // 데이터 로딩은 그대로 유지
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

    private void GetCostData()
    {
        CostDataDict = new Dictionary<ResourceType, int>();
        foreach (var cost in Data.Cost)
        {
            CostDataDict.Add(cost.Type, cost.Amount);
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
}