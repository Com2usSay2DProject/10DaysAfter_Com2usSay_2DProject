using UnityEngine;

public class EnemyDeadState : EnemyState
{
    SpriteRenderer _spriteRenderer;
    private float fadeDuration = 4f;
    private float elapsed;
    private Color originalColor;
    public EnemyDeadState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName, SpriteRenderer spriteRenderer) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
        _spriteRenderer = spriteRenderer;
    }

    public override void Enter()
    {
        base.Enter();

        _enemyBase.IsDead = true;
        elapsed = 0;

        _rigidbody.linearVelocity = Vector3.zero;

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (_triggerCalled)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            if (elapsed >= fadeDuration)
            {
                _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
                EnemyPoolManager.Instance.ReturnObject(_enemyBase.gameObject, _enemyBase.EnemyType);
            }
        }
    }

}
