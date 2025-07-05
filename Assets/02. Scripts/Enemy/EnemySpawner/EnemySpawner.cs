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
    [Header("Spawner Settings")]
    [SerializeField] private float spawnRadius = 3f;

    private List<PathSet> pathSetList = new List<PathSet>();
    private GameObject mainTowerTarget;

    private const int PATH_CACHE_COUNT = 5;

    private void OnEnable()
    {
        SetPath();
    }

    public bool Spawn(EEnemyType type)
    {
        if (!IsValidToSpawn()) return false;

        Enemy enemy = GetEnemyFromPool(type);
        if (enemy == null) return false;

        PathSet selectedPath = GetRandomPath();
        SetupEnemy(enemy, selectedPath);

        return true;
    }

    public bool SpawnEnemyCluster(int enemyNum, float radius)
    {
        if (enemyNum <= 0 || mainTowerTarget == null) return false;

        List<Enemy> enemies = CreateClusterEnemies(enemyNum);
        if (enemies.Count == 0) return false;

        Queue<Vector3> sharedPath = CalculateClusterPath();
        if (sharedPath == null) return false;

        PositionEnemiesInCluster(enemies, radius, sharedPath);
        return true;
    }

    public void SetPath()
    {
        pathSetList.Clear();

        if (!FindMainTowerTarget()) return;

        CacheMultiplePaths();
    }

    #region Private Methods

    private bool IsValidToSpawn()
    {
        return pathSetList.Count > 0 && mainTowerTarget != null;
    }

    private Enemy GetEnemyFromPool(EEnemyType type)
    {
        GameObject enemyObj = EnemyPoolManager.Instance.GetObject(type);
        return enemyObj?.GetComponent<Enemy>();
    }

    private PathSet GetRandomPath()
    {
        int randIndex = Random.Range(0, pathSetList.Count);
        return pathSetList[randIndex];
    }

    private void SetupEnemy(Enemy enemy, PathSet pathSet)
    {
        enemy.transform.position = pathSet.startPosition;
        enemy.Path = new Queue<Vector3>(pathSet.pathToMainTower);
        enemy.AttackTarget = mainTowerTarget;
    }

    private List<Enemy> CreateClusterEnemies(int count)
    {
        List<Enemy> enemies = new List<Enemy>(count);

        for (int i = 0; i < count; i++)
        {
            Enemy enemy = GetEnemyFromPool(EEnemyType.Crawler);
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }

        return enemies;
    }

    private Queue<Vector3> CalculateClusterPath()
    {
        GameObject target = EnemyTargetSelector.FindTarget(transform.position, ETargetType.MainTower);
        if (target == null) return null;

        List<Vector3> path = Pathfinding.FindPath(transform.position, target.transform.position);
        return path != null && path.Count > 0 ? new Queue<Vector3>(path) : null;
    }

    private void PositionEnemiesInCluster(List<Enemy> enemies, float radius, Queue<Vector3> path)
    {
        foreach (Enemy enemy in enemies)
        {
            Vector3 clusterOffset = GetUniformCirclePosition(radius);
            enemy.transform.position = transform.position + clusterOffset;
            enemy.Path = new Queue<Vector3>(path);
        }
    }

    private bool FindMainTowerTarget()
    {
        mainTowerTarget = EnemyTargetSelector.FindTarget(transform.position, ETargetType.MainTower);

        if (mainTowerTarget == null)
        {
            Debug.LogWarning($"[{gameObject.name}] 메인 타워를 찾지 못했습니다. 경로 설정 중단.");
            return false;
        }

        return true;
    }

    private void CacheMultiplePaths()
    {
        for (int i = 0; i < PATH_CACHE_COUNT; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            List<Vector3> path = Pathfinding.FindPath(spawnPos, mainTowerTarget.transform.position);

            if (path != null && path.Count > 0)
            {
                PathSet pathSet = new PathSet
                {
                    startPosition = spawnPos,
                    pathToMainTower = new Queue<Vector3>(path)
                };
                pathSetList.Add(pathSet);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 circleOffset = GetUniformCirclePosition(spawnRadius);
        return transform.position + circleOffset;
    }

    private Vector3 GetUniformCirclePosition(float radius)
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float t = Random.Range(0f, 1f);
        float distance = Mathf.Sqrt(t) * radius;

        return new Vector3(
            Mathf.Cos(angle) * distance,
            Mathf.Sin(angle) * distance,
            0f
        );
    }

    #endregion

}
