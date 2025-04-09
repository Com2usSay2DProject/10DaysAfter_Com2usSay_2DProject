using UnityEngine;
using System.Linq;

public class Missile : MonoBehaviour
{
    [Header("# Stat")]
    public EBulletType Type;
    [SerializeField] private float _speed;
    [SerializeField] private float _explodeRange;
    public float Damage;

    [Header ("# Bezier")]
    public Vector3 TargetPosition;
    Vector2[] point = new Vector2[4];
    [SerializeField][Range(0, 1)] private float _t = 0;
    [SerializeField] public float posA = 0.55f;
    [SerializeField] public float posB = 0.45f;
    private Vector2 _prevPos;
    private bool _isPathSet = false;

    [Header("# Effect")]
    [SerializeField] private GameObject _explodeEffect;

    private void SetPath()
    {
        _isPathSet = true;

        point[0] = transform.position; // P0
        point[1] = PointSetting(transform.position); // P1
        point[2] = PointSetting(TargetPosition); // P2
        point[3] = TargetPosition; // P3

        _prevPos = transform.position;
    }

    private void OnDisable()
    {
        _t = 0;
        _isPathSet = false;
    }

    private void Update()
    {
        if (!_isPathSet)
        {
            SetPath();
        }

        if (_t > 1)
        {
            //_t = 0;  // 새로운 베지어 경로를 생성할 준비
            //point[0] = transform.position;
            //point[1] = PointSetting(transform.position);
            //point[2] = PointSetting(TargetPosition);

            //폭발
            Explode();
        }
        _t += Time.deltaTime * _speed;

        point[3] = TargetPosition; // P3

        DrawTrajectory();
    }

    private void Explode()
    {
        // TODO : 폭발 데미지 주기
        GameObject[] Enemys = Physics2D
            .OverlapCircleAll(transform.position, _explodeRange, 1 << LayerMask.NameToLayer("Enemy"))
            .Select(c => c.gameObject).ToArray();

        foreach(GameObject enemy in Enemys)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            e?.TakeDamage(Damage);
        }

        //TODO : 폭발 이펙트
        Instantiate(_explodeEffect, transform.position, Quaternion.identity);

        BulletPoolManager.Instance.ReturnObject(gameObject, Type);
    }

    Vector2 PointSetting(Vector2 origin)
    {
        float x, y;

        x = posA * Mathf.Cos(Random.Range(0, 360) * Mathf.Deg2Rad)
        + origin.x;
        y = posB * Mathf.Sin(Random.Range(0, 360) * Mathf.Deg2Rad)
        + origin.y;
        return new Vector2(x, y);
    }

    void DrawTrajectory()
    {
        Vector2 currentPos = new Vector2(
            FourPointBezier(point[0].x, point[1].x, point[2].x, point[3].x),
            FourPointBezier(point[0].y, point[1].y, point[2].y, point[3].y)
        );

        Vector2 direction = (currentPos - _prevPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        transform.position = currentPos;

        _prevPos = currentPos;
    }

    private float FourPointBezier(float a, float b, float c, float d)
    {
        return Mathf.Pow((1 - _t), 3) * a
        + Mathf.Pow((1 - _t), 2) * 3 * _t * b
        + Mathf.Pow(_t, 2) * 3 * (1 - _t) * c
        + Mathf.Pow(_t, 3) * d;
    }
}
