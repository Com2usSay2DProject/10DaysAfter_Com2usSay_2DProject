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
            Enemy enemy = Instantiate(EnemyPrefab, transform).GetComponent<Enemy>();
            enemy.Path = _path;
        }

    }
}
