using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using System;

public enum Direction8
{
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight
}

[Serializable]
public struct DirectionSprite
{
    public Direction8 direction;
    public GameObject sprite;
}

public class AttackTower : TowerRoot
{
    [Header("# Attack, Detect")]
    [SerializeField]
    private Enemy _targetEnemy;
    private bool _isEnemyDetected;

    [Header("# Turret")]
    [SerializeField] private DirectionSprite[] directionSprites;
    private Dictionary<Direction8, GameObject> _turretDict;
    [SerializeField]
    private GameObject _turretObject;
    [SerializeField]
    private GameObject _fireEffect;

    [Header("# UniRx")]
    private IDisposable _targetEnemySubscription;

    private float _timer = 0f;
    #region Initialize
    protected override void Awake()
    {
        base.Awake();

        _turretDict = new Dictionary<Direction8, GameObject>();

        foreach (var entry in directionSprites)
        {
            _turretDict[entry.direction] = entry.sprite;
        }
    }

    public override void SetPosition()
    {
        base.SetPosition();

        _turretObject.GetComponent<SpriteRenderer>().sortingOrder = _spriteRenderer.sortingOrder + 1;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        ObserveTargetEnemy();
    }

    private void ObserveTargetEnemy()
    {
        _targetEnemySubscription?.Dispose(); // 중복 구독 방지
        _targetEnemySubscription = this.ObserveEveryValueChanged(_ => _targetEnemy)
            .Where(target => target == null)
            .Subscribe(_ =>
            {
                Debug.Log("타워 : 타겟 사라짐");
                _isEnemyDetected = false;
            }).AddTo(this);
    }
    #endregion

    protected override void Update()
    {
        base.Update();

        _timer += Time.deltaTime;

        if (!_isEnemyDetected || _targetEnemy.IsDead)
        {
            _targetEnemy = DetectEnemy()?.GetComponent<Enemy>();
            _isEnemyDetected = _targetEnemy != null;
        }
        else
        {
            Attack();
        }
    }

    private GameObject DetectEnemy()
    {
        GameObject target = null;
        float minDistance = float.MaxValue;

        GameObject[] Enemys = Physics2D
            .OverlapCircleAll(transform.position, _range, 1 << LayerMask.NameToLayer("Enemy"))
            .Select(c => c.gameObject).ToArray();

        foreach (GameObject enemy in Enemys)
        {
            float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
            if (currentDistance <= _range && currentDistance <= minDistance)
            {
                minDistance = currentDistance;
                target = enemy;
            }
        }
        if (target != null)
        {
            _isEnemyDetected = true;
        }

        return target;
    }

    private void Attack()
    {
        if (_targetEnemy == null) return;
        if (_timer < _atkSpeed) return;

        Vector2 dir = ((Vector2)_targetEnemy.transform.position - (Vector2)transform.position).normalized;
        Direction8 direction = GetDirection8(dir);

        // 방향에 맞는 스프라이트 교체
        if (_turretDict.TryGetValue(direction, out GameObject sprite))
        {
            _turretObject.SetActive(false);
            sprite.SetActive(true);
            _turretObject = sprite;
            _turretObject.GetComponent<SpriteRenderer>().sortingOrder = _spriteRenderer.sortingOrder + 1;
            _fireEffect = _turretObject.transform.GetChild(0).gameObject;
        }

        // 필요 시: 해당 방향으로 총알 발사 등
        //_fireEffect.SetActive(true);
        var ps = _fireEffect.GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play();

        _targetEnemy.GetComponent<Enemy>().TakeDamage(_damage);

        _timer = 0f;
    }

    private Direction8 GetDirection8(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle + 360f) % 360f;

        if (angle >= 337.5f || angle < 22.5f)
            return Direction8.Right;
        else if (angle >= 22.5f && angle < 67.5f)
            return Direction8.UpRight;
        else if (angle >= 67.5f && angle < 112.5f)
            return Direction8.Up;
        else if (angle >= 112.5f && angle < 157.5f)
            return Direction8.UpLeft;
        else if (angle >= 157.5f && angle < 202.5f)
            return Direction8.Left;
        else if (angle >= 202.5f && angle < 247.5f)
            return Direction8.DownLeft;
        else if (angle >= 247.5f && angle < 292.5f)
            return Direction8.Down;
        else
            return Direction8.DownRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}