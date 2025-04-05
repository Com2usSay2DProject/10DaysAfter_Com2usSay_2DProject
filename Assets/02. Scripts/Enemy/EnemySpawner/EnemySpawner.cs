using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyTargetSelector targetSelector;

    private Queue<Vector3> _pathNomal;
    private Queue<Vector3> _pathTowrTarget;

    private void Start()
    {
        //PhaseManager.Instance.OnDayBegin += SetPath;
        SetPath();
    }

    public void Spawn(EEnemyType type)
    {
        Enemy enemy = EnemyPoolManager.Instance.GetObject(type).GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.transform.position = transform.position;

            switch (type)
            {
                case EEnemyType.NomalEnemy:
                    enemy.Path = _pathNomal;
                    break;
                case EEnemyType.TowerAttackEnemy:
                    enemy.Path = _pathTowrTarget;
                    break;
            }

        }
    }
    private void SetPath()
    {
        List<Vector3> nomalPath = Pathfinding.FindPath(transform.position, targetSelector.FindTarget(TargetType.MainTower).position);
        List<Vector3> towerTargetPath = Pathfinding.FindPath(transform.position, targetSelector.FindTarget(TargetType.Tower).position);

        if(nomalPath.Count>0) _pathNomal = new Queue<Vector3>(nomalPath);

        if(towerTargetPath.Count>0) _pathTowrTarget = new Queue<Vector3>(towerTargetPath);
    }

}
