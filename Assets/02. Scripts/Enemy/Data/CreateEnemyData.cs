using System.Collections.Generic;
using UnityEngine;

public class CreateEnemyData : MonoBehaviour
{
    EnemyDataCollection collection = new EnemyDataCollection();

    void Start()
    {
        collection.Datas = new List<EnemyData>();

        collection.Datas.Add(new EnemyData
        {
            TargetType = ETargetType.MainTower,
            EnemyType = EEnemyType.NomalEnemy,
            TypeString = "NomalEnemy",
            MaxHp = 120,
            Speed = 1.0f,
            AtkSpeed = 0.5f,
            Damage = 15,
            Range = 0.5f
        }); 
        collection.Datas.Add(new EnemyData
        {
            TargetType = ETargetType.Tower,
            EnemyType = EEnemyType.TowerAttackEnemy,
            TypeString = "TowerAttackEnemy",
            MaxHp = 80,
            Speed = 1.5f,
            AtkSpeed = 0.6f,
            Damage = 20,
            Range = 0.5f
        });
        collection.Datas.Add(new EnemyData
        {
            TargetType = ETargetType.Tower,
            EnemyType = EEnemyType.ThrowEnemy,
            TypeString = "ThrowEnemy",
            MaxHp = 50,
            Speed = 1.0f,
            AtkSpeed = 0.6f,
            Damage = 10,
            Range = 2.5f
        });
        collection.Datas.Add(new EnemyData
        {
            TargetType = ETargetType.Tower,
            EnemyType = EEnemyType.Boomer,
            TypeString = "Boomer",
            MaxHp = 100,
            Speed = 1.0f,
            AtkSpeed = 1,
            Damage = 50,
            Range = 1f
        });
        collection.Datas.Add(new EnemyData
        {
            TargetType = ETargetType.MainTower,
            EnemyType = EEnemyType.Crawler,
            TypeString = "Crawler",
            MaxHp = 30,
            Speed = 0.5f,
            AtkSpeed = 0.8f,
            Damage = 10,
            Range = 0.6f
        });
        JsonDataManager.CreateFile("Enemy/EnemyDataCollection", collection);
    }

    void Update()
    {

    }
}
