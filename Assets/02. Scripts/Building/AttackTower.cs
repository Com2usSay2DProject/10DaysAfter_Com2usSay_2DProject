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

[System.Serializable]
public struct DirectionSprite
{
    public Direction8 direction;
    public Sprite sprite;
}

public class AttackTower : TowerRoot
{
    [Header("# Attack, Detect")]
    private GameObject _targetEnemy;
    private bool _isEnemyDetected;

    [Header("# Turret")]
    [SerializeField] private DirectionSprite[] directionSprites;
    [SerializeField] private GameObject Turret;
    private Dictionary<Direction8, Sprite> _spriteDict;
    private SpriteRenderer _turretSprite;

    [Header("# UniRx")]
    private IDisposable _targetEnemySubscription;

    protected override void Awake()
    {
        base.Awake();

        _turretSprite = Turret.GetComponent<SpriteRenderer>();
        _spriteDict = new Dictionary<Direction8, Sprite>();

        foreach (var entry in directionSprites)
        {
            _spriteDict[entry.direction] = entry.sprite;
        }
    }

    public override void SetPosition()
    {
        base.SetPosition();

        _turretSprite.sortingOrder = _spriteRenderer.sortingOrder + 1;
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

    private void Update()
    {
        if (UIManager.Instance.isBuildModeActive && !IsBuilt)
        {
            _spriteRenderer.sortingOrder = 1000;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            _rigid.MovePosition(mousePos);
        }

        if (!_isEnemyDetected || !_targetEnemy)
        {
            _targetEnemy = DetectEnemy();
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

        Vector2 dir = ((Vector2)_targetEnemy.transform.position - (Vector2)transform.position).normalized;
        Direction8 direction = GetDirection8(dir);

        // 방향에 맞는 스프라이트 교체
        if (_spriteDict.TryGetValue(direction, out Sprite sprite))
        {
            _turretSprite.sprite = sprite;
        }

        // 필요 시: 해당 방향으로 총알 발사 등
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
}
