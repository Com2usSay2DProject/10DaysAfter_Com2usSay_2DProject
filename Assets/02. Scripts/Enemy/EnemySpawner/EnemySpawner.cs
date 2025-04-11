using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class PathSet
{
    public Vector3 startPosition;
    public Queue<Vector3> pathToMainTower;
    public Queue<Vector3> pathToTower;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyTargetSelector targetSelector;

    private List<PathSet> _pathSetList = new List<PathSet>();


    private GameObject TowrTarget;
    private GameObject MainTowerTarget;
    private void Start()
    {
        //PhaseManager.Instance.OnNightBegin += SetPath;
        //SetPath();
    }
    private void OnEnable()
    {
        //SetPath();
        //_pathNomal.Clear();
        //_pathTowrTarget.Clear();
        //SetPath();
    }

    public void Spawn(EEnemyType type)
    {
        Enemy enemy = EnemyPoolManager.Instance.GetObject(type).GetComponent<Enemy>();
        if (enemy == null || _pathSetList.Count == 0) return;

        //PathSet 중 랜덤하게 하나 선택
        int randIndex = Random.Range(0, _pathSetList.Count);
        PathSet selected = _pathSetList[randIndex];

        //출발 위치 적용
        enemy.transform.position = selected.startPosition;

        //경로 및 타겟 설정
        switch (type)
        {
            case EEnemyType.NomalEnemy:
                enemy.Path = new Queue<Vector3>(selected.pathToMainTower);
                enemy.AttackTerget = MainTowerTarget;
                break;
            case EEnemyType.TowerAttackEnemy:
                if (selected.pathToTower != null)
                {
                    enemy.Path = new Queue<Vector3>(selected.pathToTower);
                    enemy.AttackTerget = TowrTarget;
                }
                else
                {
                    enemy.Path = new Queue<Vector3>(selected.pathToMainTower);
                    enemy.AttackTerget = MainTowerTarget;
                }
                break;
            case EEnemyType.Boomer:
                if (selected.pathToTower != null)
                {
                    enemy.Path = new Queue<Vector3>(selected.pathToTower);
                    enemy.AttackTerget = TowrTarget;
                }
                else
                {
                    enemy.Path = new Queue<Vector3>(selected.pathToMainTower);
                    enemy.AttackTerget = MainTowerTarget;
                }
                break;
            case EEnemyType.ThrowEnemy:
                if (selected.pathToTower != null)
                {
                    enemy.Path = new Queue<Vector3>(selected.pathToTower);
                    enemy.AttackTerget = TowrTarget;
                }
                else
                {
                    enemy.Path = new Queue<Vector3>(selected.pathToMainTower);
                    enemy.AttackTerget = MainTowerTarget;
                }
                break;

        }

    }
    public void SpawnEnemyCluster(int enemyNum, float radius)
    {
        List<Enemy> enemies = new List<Enemy>();
        enemies.Capacity = enemyNum;

        List<Vector3> path = Pathfinding.FindPath(transform.position, targetSelector.FindTarget(ETargetType.MainTower).transform.position);
        Queue<Vector3> pathQue = new Queue<Vector3>(path);
        for (int i = 0; i < enemyNum; ++i)
        {
            enemies.Add(EnemyPoolManager.Instance.GetObject(EEnemyType.Crawler).GetComponent<Enemy>());
        }

        foreach (var enemy in enemies)
        {
            // 원 안에 랜덤 위치 생성 (균일 분포)
            float angle = Random.Range(0f, 2 * Mathf.PI);
            float t = Random.Range(0f, 1f);
            float randRadius = Mathf.Sqrt(t) * radius;

            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * randRadius;
            enemy.transform.position = transform.position + offset;
            enemy.Path = pathQue;
        }


    }

    public void SetPath()
    {

        _pathSetList.Clear();

        MainTowerTarget = targetSelector.FindTarget(ETargetType.MainTower);
        TowrTarget = targetSelector.FindTarget(ETargetType.Tower);

        for (int i = 0; i < 9; i++)
        {
            // 랜덤 위치 생성
            float angle = Random.Range(0f, 2 * Mathf.PI);
            float t = Random.Range(0f, 1f);
            float randRadius = Mathf.Sqrt(t) * 2f;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * randRadius;
            Vector3 spawnPos = transform.position + offset;

            PathSet pathSet = new PathSet();
            pathSet.startPosition = spawnPos;

            // MainTower 경로
            if (MainTowerTarget != null)
            {
                var path = Pathfinding.FindPath(spawnPos, MainTowerTarget.transform.position);
                if (path != null && path.Count > 0)
                    pathSet.pathToMainTower = new Queue<Vector3>(path);
            }

            // TowrTarget 경로
            if (TowrTarget != null)
            {
                var path = Pathfinding.FindPath(spawnPos, TowrTarget.transform.position);
                if (path != null && path.Count > 0)
                    pathSet.pathToTower = new Queue<Vector3>(path);
            }

            _pathSetList.Add(pathSet);
        }

    }

}
