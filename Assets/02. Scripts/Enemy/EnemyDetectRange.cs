using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectRange : MonoBehaviour
{
    Enemy _enemy;
    CircleCollider2D _circleCollider;
    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower"))
        {
            Debug.Log("ReTargeting");

            GameObject towerTarget = EnemyTargetSelector.FindTarget(_enemy.transform.position, ETargetType.Tower);
            if (towerTarget == null) return;

            List<Vector3> path = Pathfinding.FindPath(_enemy.transform.position, towerTarget.transform.position);
            if (path != null && path.Count > 0)
            {
                _enemy.Path = new Queue<Vector3>(path);
                _enemy.AttackTarget = towerTarget;
                _enemy.HasTowerInRange = true;
            }
        }
    }
}
