using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRoot : Building
{
    private static Dictionary<ETowerType, TowerData> _towerDataDict;

    [Header("# Stats")]
    public ETowerType TowerType;
    [SerializeField] protected TowerData Data;
    protected float _maxHp;
    protected float _hp;
    protected float _damage;
    protected float _atkSpeed;
    protected float _range;

    [Header("# Cost")]
    public Dictionary<ResourceType, int> CostDataDict { get; private set; }

    [Header("# Effect")]
    [SerializeField] private GameObject _buildEffect;

    protected override void Awake()
    {
        base.Awake();
        GetData();
        GetDataForThis();
        GetCostData();
        ResourceManager.Instance.OnPopulationChange += MultiplyData;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        MultiplyData();
        _collider.enabled = false;
        _spriteRenderer.color = _tempColor;
    }

    protected override void OnPlaced()
    {
        _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        StartCoroutine(CoBuildRoutine());
    }

    private IEnumerator CoBuildRoutine()
    {
        float timer = 0f;

        while(timer < Data.BuildTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _spriteRenderer.color = Color.white;
        IsPlaced = true;
    }

    public void TakeDamage(float damage)
    {
        _hp -= damage;
        if (_hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 타일을 Empty로 변경
        GridBuildingSystem.Instance.ClearArea(GetGridArea());

        ResourceManager.Instance.TryUseResource(ResourceType.Population, 10);
        GameObject explode = EffectPoolManager.Instance.GetObject(EEffectType.BuildingExplode);
        explode.transform.position = transform.position;
        SoundManager.Instance.PlaySfx(ESfxType.BuildingExplode);
        TowerPoolManager.Instance.ReturnObject(gameObject, TowerType);
    }

    #region Data
    private void GetData()
    {
        if (_towerDataDict == null)
        {
            TowerDataCollection collection = JsonDataManager.LoadFromFile<TowerDataCollection>("Tower/TowerDataCollection");
            _towerDataDict = new Dictionary<ETowerType, TowerData>();

            foreach (TowerData d in collection.Datas)
            {
                d.TypeString = d.TowerType.ToString();
                _towerDataDict[d.TowerType] = d;
            }
        }
    }

    private void GetDataForThis()
    {
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
        _range = Data.GetModifiedStat(Data.Range, ResourceManager.Instance.GetResourceAmount(ResourceType.Population));
    }
    #endregion
}