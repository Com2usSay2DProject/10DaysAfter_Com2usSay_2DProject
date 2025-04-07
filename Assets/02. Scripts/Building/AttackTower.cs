using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private DirectionSprite[] directionSprites;
    [SerializeField] private GameObject Turret;

    private Dictionary<Direction8, Sprite> _spriteDict;
    private SpriteRenderer _turretSprite;

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

    protected override void Attack()
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
