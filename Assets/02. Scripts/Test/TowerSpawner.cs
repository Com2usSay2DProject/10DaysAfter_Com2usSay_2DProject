using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSpawner : Singleton<TowerSpawner>
{
    [SerializeField]
    private GameObject _tower;

    public Vector3 TowerOffset;

    private void Awake()
    {
        TowerDataCollection collection = new TowerDataCollection();
        collection.Datas.Add(new TowerData
        {
            TowerType = ETowerType.TempTower,
            TypeString = ETowerType.TempTower.ToString(),
            MaxHp = 100f,
            Damage = 10f,
            Range = 3f,
            AtkSpeed = 1f,
            Cost = new System.Collections.Generic.List<TowerCostData>()
        });
        collection.Datas[0].Cost.Add(new TowerCostData(ResourceType.Wood, 1));
        collection.Datas[0].Cost.Add(new TowerCostData(ResourceType.Stone, 2));
        JsonDataManager.CreateFile("Tower/TowerDataCollection", collection);
        /*TowerCostDataCollection collection = new TowerCostDataCollection();
        collection.Datas.Add(new TowerCostData
        {
            TowerType = ETowerType.TempTower,
            TowerTypeString = ETowerType.TempTower.ToString(),
            Cost = 100,
        });
        JsonDataManager.CreateFile("Tower/TowerCostDataCollection", collection);*/
    }

    private void Start()
    {
        //UIManager.Instance.BuildModeActivate += () => SpawnTower();
    }

    public GameObject SpawnTower(Vector3 tilePosition = default, ETowerType type = ETowerType.TempTower)
    {
        GameObject tower = TowerPoolManager.Instance.GetObject(type);
        tower.transform.position = tilePosition + TowerOffset;
        return tower;
    }
}