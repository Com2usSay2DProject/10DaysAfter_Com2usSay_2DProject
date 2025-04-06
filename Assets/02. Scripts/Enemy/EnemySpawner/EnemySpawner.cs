using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyTargetSelector targetSelector;

    private Queue<Vector3> _pathNomal = new Queue<Vector3>();
    private Queue<Vector3> _pathTowrTarget = new Queue<Vector3>();

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
        if (nomalPath.Count > 0) _pathNomal = new Queue<Vector3>(nomalPath);


        if (targetSelector.FindTarget(TargetType.Tower) != null)
        {
            Vector3 TargetPos = targetSelector.FindTarget(TargetType.Tower).position;
            TargetPos.z = 0;
            List<Vector3> towerTargetPath = Pathfinding.FindPath(transform.position, TargetPos);

            if(towerTargetPath != null)
                if(towerTargetPath.Count > 0) _pathTowrTarget = new Queue<Vector3>(towerTargetPath);
        }


    }

}
