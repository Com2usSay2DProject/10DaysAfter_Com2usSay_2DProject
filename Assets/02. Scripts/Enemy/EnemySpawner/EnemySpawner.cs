using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyTargetSelector targetSelector;

    private Queue<Vector3> _pathNomal = new Queue<Vector3>();
    private Queue<Vector3> _pathTowrTarget = new Queue<Vector3>();

    private GameObject TowrTarget;
    private GameObject MainTowerTarget;
    private void Start()
    {
        //PhaseManager.Instance.OnNightBegin += SetPath;
        SetPath();
    }
    private void OnEnable()
    {
        _pathNomal.Clear();
        _pathTowrTarget.Clear();
        SetPath();
    }

    public void Spawn(EEnemyType type)
    {
        Enemy enemy = EnemyPoolManager.Instance.GetObject(type).GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.transform.position = transform.position;

            if (_pathTowrTarget.Count <= 0)
            {
                enemy.Path = _pathNomal;
                return;
            }
            switch (type)
            {
                case EEnemyType.NomalEnemy:
                    enemy.Path = _pathNomal;
                    enemy.AttackTerget = MainTowerTarget;
                    break;
                case EEnemyType.TowerAttackEnemy:
                    enemy.Path = _pathTowrTarget;
                    enemy.AttackTerget = TowrTarget;
                    break;
                case EEnemyType.Boomer:
                    enemy.Path = _pathTowrTarget;
                    enemy.AttackTerget = TowrTarget;
                    break;
            }

        }
    }
    public void SpawnEnemyCluster(int enemyNum, float radius)
    {
        List<Enemy> enemies = new List<Enemy>();
        enemies.Capacity = enemyNum;
        for(int i=0; i<enemyNum;++i)
        {
            enemies.Add(EnemyPoolManager.Instance.GetObject(EEnemyType.Crawler).GetComponent<Enemy>());
        }

        foreach(var enemy in enemies)
        {
            // 원 안에 랜덤 위치 생성 (균일 분포)
            float angle = Random.Range(0f, 2 * Mathf.PI);
            float t = Random.Range(0f, 1f);
            float randRadius = Mathf.Sqrt(t) * radius;

            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * randRadius;
            enemy.transform.position = transform.position + offset;
            enemy.Path = _pathNomal;
        }


    }

    private void SetPath()
    {
        MainTowerTarget = targetSelector.FindTarget(ETargetType.MainTower);
        List<Vector3> nomalPath = Pathfinding.FindPath(transform.position, MainTowerTarget.transform.position);
        if (nomalPath.Count > 0) _pathNomal = new Queue<Vector3>(nomalPath);

        TowrTarget = targetSelector.FindTarget(ETargetType.Tower);

        if (TowrTarget != null)
        {
            Vector3 TargetPos = TowrTarget.transform.position;
            List<Vector3> towerTargetPath = Pathfinding.FindPath(transform.position, TargetPos);

            if(towerTargetPath != null)
                if(towerTargetPath.Count > 0) _pathTowrTarget = new Queue<Vector3>(towerTargetPath);
        }


    }

}
