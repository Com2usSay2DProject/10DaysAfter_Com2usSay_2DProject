using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private TargetType _priorityTarget;
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    private EnemyTargetSelector _targetSelector;

    private Transform Target;

    private Queue<Vector3> _path;

    float _spawnTimer;
    [SerializeField] private float _spawnTime;


    protected virtual void Awake()
    {
        _targetSelector = GetComponent<EnemyTargetSelector>();

    }

    private void ActiveSpawner() { gameObject.SetActive(true); }
    private void DisActiveSpawner() { gameObject.SetActive(false); }

    protected virtual void Start()
    {
        if (PhaseManager.Instance)
        {
            PhaseManager.Instance.OnNightBegin += ActiveSpawner;
            PhaseManager.Instance.OnNightEnd += DisActiveSpawner;
        }
        Target = _targetSelector.FindTarget(_priorityTarget);
        if(Target==null)
        {
            Debug.Log("메인건물로 셋팅");
            Target = _targetSelector.FindTarget(TargetType.MainTower);
        }

        transform.position = GetRandomPositionOutsideRadius(Vector2.zero, minDistance, maxDistance);
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
    protected virtual void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer < 0)
        {
            _spawnTimer = Random.Range(_spawnTime, _spawnTime * 2);
            Enemy enemy = PoolManager.Instance.GetObject(EObjectType.NomalEnemy).GetComponent<Enemy>();
            enemy.transform.position = transform.position;
            enemy.Path = _path;
        }

    }

    protected virtual private Vector2 GetRandomPositionOutsideRadius(Vector2 center, float minDistance, float maxDistance)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float distance = Mathf.Lerp(minDistance, maxDistance, Mathf.Sqrt(Random.Range(0f, 1f))); // 거리: min ~ max
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        return center + offset;
    }

}
