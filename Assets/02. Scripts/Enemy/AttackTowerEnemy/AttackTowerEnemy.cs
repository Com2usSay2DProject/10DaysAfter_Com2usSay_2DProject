using System.Collections.Generic;
using UnityEngine;

public class AttackTowerEnemy : Enemy
{
    private EnemyTargetSelector _targetSelector;


    protected override void Awake()
    {
        base.Awake();
    }


    protected override void Start()
    {
        base.Start();
    }

    protected override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Update()
    {
        base.Update();
        

    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            _stateMachine.ChangeState(AttackState);
            HasTowerInRange = true;

        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        //Transform Target =_targetSelector.FindTarget(TargetType.Tower);

        //List<Vector3> pathList = Pathfinding.FindPath(gameObject.transform.position, Target.position);
        //if (pathList != null && pathList.Count > 0)
        //{
        //    Path = new Queue<Vector3>(pathList);
        //}
        //else
        //{
        //    Target = _targetSelector.FindTarget(TargetType.MainTower);
        //    pathList = Pathfinding.FindPath(gameObject.transform.position, Target.position);
        //    if (pathList != null && pathList.Count > 0)
        //    {
        //        Path = new Queue<Vector3>(pathList);
        //    }
        //    else
        //    {
        //        Debug.Log("경로를 찾을수 없습니다.");
        //    }
        //}

    }
}
