using System.Collections;
using UnityEngine;

public class MissileTower : AttackTower
{
    protected override void Attack()
    {
        SetMissileTarget();
    }

    private void SetMissileTarget()
    {
        Vector2[] targets = new Vector2[6];

        //TODO: targetEnemy 주변으로 랜덤한 위치들 적용
        for(int i=0; i<targets.Length; i++)
        {
            targets[i] = (Vector2)_targetEnemy.transform.position + Random.insideUnitCircle * 3;
        }

        StartCoroutine(FireMissile(targets));
    }

    private IEnumerator FireMissile(Vector2[] targets)
    {
        foreach (Vector2 target in targets)
        {
            Missile missile = BulletPoolManager.Instance.GetObject(EBulletType.Missile).GetComponent<Missile>();
            missile.Damage = _damage;
            missile.transform.position = _turretObject.transform.position;
            missile.TargetPosition = target;

            yield return new WaitForSeconds(0.2f);
        }
    }
}
