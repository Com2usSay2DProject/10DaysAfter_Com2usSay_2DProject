using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
{
    Enemy _enemy;
    CircleCollider2D _circleCollider;
    private void Awake()
    {
        _enemy = GetComponentInParent<Enemy>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }
    void Start()
    {
        _circleCollider.radius = _enemy.AttackRange;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("MainTower") || collision.CompareTag("Tower"))
        {
            _enemy.CanAttack();
            _enemy.AttackTarget  = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _enemy.HasTowerInRange = false;
    }
}
