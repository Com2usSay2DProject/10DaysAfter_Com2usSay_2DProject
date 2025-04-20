using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class PathSet
{
    public Vector3 startPosition;
    public Queue<Vector3> pathToMainTower;
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _spawnRadius = 3f; // 스포너 반경

    private List<PathSet> _pathSetList = new List<PathSet>();

    private GameObject MainTowerTarget;

    private void OnEnable()
    {
        SetPath();
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
        enemy.Path = new Queue<Vector3>(selected.pathToMainTower);
        enemy.AttackTarget = MainTowerTarget;

    }
    public void SpawnEnemyCluster(int enemyNum, float radius)
    {
        List<Enemy> enemies = new List<Enemy>();
        enemies.Capacity = enemyNum;

        List<Vector3> path = Pathfinding.FindPath(transform.position, EnemyTargetSelector.FindTarget(transform.position,ETargetType.MainTower).transform.position);
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

        MainTowerTarget = EnemyTargetSelector.FindTarget(transform.position,ETargetType.MainTower);
        if (MainTowerTarget == null)
        {
            Debug.LogWarning("[EnemySpawner] 메인 타워를 찾지 못했습니다. 경로 설정 중단.");
            return;
        }

        for (int i = 0; i < 5; i++)
        {
            // 랜덤 위치 생성
            float angle = Random.Range(0f, 2 * Mathf.PI);
            float t = Random.Range(0f, 1f);
            float randRadius = Mathf.Sqrt(t) * 2f;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _spawnRadius;
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
            _pathSetList.Add(pathSet);
        }

    }

}
