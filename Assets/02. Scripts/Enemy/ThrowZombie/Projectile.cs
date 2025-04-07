using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed;
    Enemy _owner;
    float _damage;
    Vector3 _dir;

    Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_owner.gameObject.activeSelf == false)
        {
            Destroy(gameObject);
        }

        _rigidbody.linearVelocity = _dir * _speed;
        //Vector3 Velocity = _dir * _speed * Time.deltaTime;
        //transform.position += Velocity;

    }

    public void Init(Enemy enemy, float damage, Vector3 dir)
    {
        _owner = enemy;
        _damage = damage;
        _dir = dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("MainTower") || collision.CompareTag("Tower"))
        {
            TowerRoot Tower = collision.GetComponent<TowerRoot>();

            if (Tower == null) return;

            Tower.TakeDamage(_damage);

            Destroy(gameObject);
            Debug.Log("Attack Damage");
        }
    }
}
