using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRoot : Building
{
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
        ResourceManager.Instance.OnPopulationChange += UpdateStats;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        Data = TowerDataManager.Instance.GetTowerData(TowerType);
        if (Data == null) return;

        _maxHp = Data.MaxHp;
        _hp = _maxHp;
        _damage = Data.Damage;
        _atkSpeed = Data.AtkSpeed;
        _range = Data.Range;

        CostDataDict = TowerDataManager.Instance.GetTowerCost(TowerType);
    }

    private void UpdateStats()
    {
        _maxHp = TowerDataManager.Instance.GetModifiedStat(TowerType, "MaxHp", Data.MaxHp);
        _damage = TowerDataManager.Instance.GetModifiedStat(TowerType, "Damage", Data.Damage);
        _range = TowerDataManager.Instance.GetModifiedStat(TowerType, "Range", Data.Range);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateStats();
        _collider.enabled = false;
        _spriteRenderer.color = _tempColor;
    }

    protected override void OnPlaced()
    {
        _collider.enabled = true;
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

    protected virtual void Die()
    {
        BoundsInt areaToClean = GetGridArea();
        GridBuildingSystem.Instance.ClearArea(areaToClean);

        ResourceManager.Instance.TryUseResource(ResourceType.Population, 10);
        GameObject explode = EffectPoolManager.Instance.GetObject(EEffectType.BuildingExplode);
        explode.transform.position = transform.position;
        SoundManager.Instance.PlaySfx(ESfxType.BuildingExplode);
        
        TowerPoolManager.Instance.ReturnObject(gameObject, TowerType);
    }
}