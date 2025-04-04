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
        });
        JsonDataManager.CreateFile("Tower/TowerDataCollection", collection);
    }

    public void SpawnTower(Vector3 tilePosition)
    {
        GameObject tower = TowerPoolManager.Instance.GetObject(ETowerType.TempTower);
        tower.transform.position = tilePosition + TowerOffset;
    }
}