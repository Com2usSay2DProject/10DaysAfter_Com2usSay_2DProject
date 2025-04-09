using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSpawner : Singleton<TowerSpawner>
{
    public Vector3 TowerOffset;

    private void Awake()
    {
        /*TowerDataCollection collection = new TowerDataCollection();
        collection.Datas.Add(new TowerData
        {
            TowerType = ETowerType.TempTower,
            TypeString = ETowerType.TempTower.ToString(),
            MaxHp = 400,
            Damage = 24,
            Range = 10f,
            AtkSpeed = 0.5f,
            Cost = new System.Collections.Generic.List<TowerCostData>()
        });
        collection.Datas[0].Cost.Add(new TowerCostData(ResourceType.Wood, 50));
        collection.Datas[0].Cost.Add(new TowerCostData(ResourceType.Stone, 50));

        collection.Datas.Add(new TowerData
        {
            TowerType = ETowerType.AttackTower,
            TypeString = ETowerType.AttackTower.ToString(),
            MaxHp = 400,
            Damage = 24,
            Range = 5f,
            AtkSpeed = 0.5f,
            Cost = new System.Collections.Generic.List<TowerCostData>()
        });
        collection.Datas[1].Cost.Add(new TowerCostData(ResourceType.Wood, 50));
        collection.Datas[1].Cost.Add(new TowerCostData(ResourceType.Stone, 50));

        collection.Datas.Add(new TowerData
        {
            TowerType = ETowerType.WoodTower,
            TypeString = ETowerType.WoodTower.ToString(),
            MaxHp = 500,
            Damage = 8,
            Range = 0f,
            AtkSpeed = 1f,
            Cost = new System.Collections.Generic.List<TowerCostData>()
        });
        collection.Datas[2].Cost.Add(new TowerCostData(ResourceType.Wood, 50));

        collection.Datas.Add(new TowerData
        {
            TowerType = ETowerType.StoneTower,
            TypeString = ETowerType.StoneTower.ToString(),
            MaxHp = 500,
            Damage = 8,
            Range = 0f,
            AtkSpeed = 1f,
            Cost = new System.Collections.Generic.List<TowerCostData>()
        });
        collection.Datas[3].Cost.Add(new TowerCostData(ResourceType.Wood, 50));

        collection.Datas.Add(new TowerData
        {
            TowerType = ETowerType.MetalTower,
            TypeString = ETowerType.MetalTower.ToString(),
            MaxHp = 500,
            Damage = 8,
            Range = 0f,
            AtkSpeed = 1f,
            Cost = new System.Collections.Generic.List<TowerCostData>()
        });
        collection.Datas[4].Cost.Add(new TowerCostData(ResourceType.Stone, 50));

        collection.Datas.Add(new TowerData
        {
            TowerType = ETowerType.FoodTower,
            TypeString = ETowerType.FoodTower.ToString(),
            MaxHp = 500,
            Damage = 8,
            Range = 0f,
            AtkSpeed = 1f,
            Cost = new System.Collections.Generic.List<TowerCostData>()
        });
        collection.Datas[5].Cost.Add(new TowerCostData(ResourceType.Metal, 50));

        collection.Datas.Add(new TowerData
        {
            TowerType = ETowerType.MissileTower,
            TypeString = ETowerType.MissileTower.ToString(),
            MaxHp = 400,
            Damage = 24,
            Range = 5f,
            AtkSpeed = 0.5f,
            Cost = new System.Collections.Generic.List<TowerCostData>()
        });
        collection.Datas[6].Cost.Add(new TowerCostData(ResourceType.Wood, 50));
        collection.Datas[6].Cost.Add(new TowerCostData(ResourceType.Stone, 50));

        JsonDataManager.CreateFile("Tower/TowerDataCollection", collection);*/
        /*TowerCostDataCollection collection = new TowerCostDataCollection();
        collection.Datas.Add(new TowerCostData
        {
            TowerType = ETowerType.TempTower,
            TowerTypeString = ETowerType.TempTower.ToString(),
            Cost = 100,
        });
        JsonDataManager.CreateFile("Tower/TowerCostDataCollection", collection);*/
    }

    public GameObject SpawnTower(Vector3 tilePosition = default, ETowerType type = ETowerType.TempTower)
    {
        GameObject tower = TowerPoolManager.Instance.GetObject(type);
        tower.transform.position = tilePosition + TowerOffset;
        return tower;
    }
}