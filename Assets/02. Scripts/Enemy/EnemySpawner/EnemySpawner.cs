using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private Transform Target;

    private Queue<Vector3> _path;

    float _spawnTimer;
    [SerializeField]private float _spawnTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        transform.position = GetRandomPositionOutsideRadius(Target.position, 4f, 5f);
        transform.rotation = Quaternion.identity;

        Vector3 startPos = transform.position;
        Vector3 targetPos = Target.position;

        List<Vector3> pathList = Pathfinding.FindPath(startPos, targetPos);
        if (pathList != null && pathList.Count > 0)
        {
            _path = new Queue<Vector3>(pathList);
        }
        else
        {
            Debug.LogError("경로를 찾을 수 없습니다.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if(_spawnTimer<0)
        {
            _spawnTimer = Random.Range(_spawnTime, _spawnTime * 2);
            //Enemy enemy = Instantiate(EnemyPrefab, transform).GetComponent<Enemy>();
            //enemy.Path = _path;
            Enemy enemy = PoolManager.Instance.GetObject(EObjectType.Enemy).GetComponent<Enemy>();
            enemy.transform.position = transform.position;
            enemy.Path = _path;
        }

    }

    private Vector2 GetRandomPositionOutsideRadius(Vector2 center, float minDistance, float maxDistance)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f); 
        float distance = Mathf.Lerp(minDistance, maxDistance, Mathf.Sqrt(Random.Range(0f, 1f))); // 거리: min ~ max
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        return center + offset;
    }
}
