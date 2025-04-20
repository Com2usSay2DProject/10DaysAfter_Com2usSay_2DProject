using UnityEngine;

public class EnemyAnimTrigger : MonoBehaviour
{
    //애니메이션 트리거용

    Enemy _enemy;

    Bommer _bommer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
        _bommer = _enemy as Bommer;
    }

    private void AnimTrigger()
    {
        _enemy.AnimTrigger();
    }

    private void NomalAttackTrigger()
    {
        if (_enemy.AttackTarget .activeSelf == false) return;

        TowerRoot Tower = _enemy.AttackTarget .GetComponent<TowerRoot>();

        if (Tower == null) return;

        if(EnemySoundManager.Instance != null)
            EnemySoundManager.Instance.Play3DSoundWithLimit(_enemy.transform.position,EnemySoundType.EnemyAttck);
        Tower.TakeDamage(_enemy.Damage);


    }

    private void BommerAttackTrigger()
    {
        if (_enemy.AttackTarget .activeSelf == false) return;

        TowerRoot Tower = _enemy.AttackTarget .GetComponent<TowerRoot>();

        if (Tower == null) return;

        Tower.TakeDamage(_enemy.Damage);


        _bommer.isAttacked = true;
    }

    private void ThrowAttckTrigger()
    {
        if (_enemy.ProjectilePrefab == null) return;

        Projectile projectile = Instantiate(_enemy.ProjectilePrefab).GetComponent<Projectile>();
        projectile.Init(_enemy, _enemy.Damage, _enemy.FaceDir);
        projectile.transform.position = _enemy.transform.position;
    }

    private void Unique2EnemyAttackTrigger()
    {
        if (_enemy.ProjectilePrefab == null) return;

        TowerRoot Tower = _enemy.AttackTarget .GetComponent<TowerRoot>();
        GameObject acid=Instantiate(_enemy.ProjectilePrefab);

        Vector3 offset = _enemy.FaceDir * 2f;
        Vector3 newpos = _enemy.transform.position + offset;
        acid.transform.position = newpos;



        if (Tower == null) return;
        Tower.TakeDamage(_enemy.Damage);
    }


}
