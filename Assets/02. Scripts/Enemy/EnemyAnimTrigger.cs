using UnityEngine;

public class EnemyAnimTrigger : MonoBehaviour
{
    Enemy _enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    private void AnimTrigger()
    {
        _enemy.AnimTrigger();
    }

    private void NomalAttackTrigger()
    {
        if (_enemy.AttackTerget.activeSelf == false) return;

        TowerRoot Tower = _enemy.AttackTerget.GetComponent<TowerRoot>();

        if (Tower == null) return;

        Tower.TakeDamage(_enemy.Damage);
        Debug.Log("Attack Damage");
    }

    private void BommerAttackTrigger()
    {
        if (_enemy.AttackTerget.activeSelf == false) return;

        TowerRoot Tower = _enemy.AttackTerget.GetComponent<TowerRoot>();

        if (Tower == null) return;

        Tower.TakeDamage(_enemy.Damage);
        Debug.Log("Attack Damage");
    }
}
