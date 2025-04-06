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
}
