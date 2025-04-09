using UnityEngine;

public class EnemyDeadState : EnemyState
{
    SpriteRenderer _spriteRenderer;
    private float fadeDuration = 1f;
    private float elapsed;
    private Color originalColor;
    CircleCollider2D circleCollider;
    public EnemyDeadState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, string animBoolName, SpriteRenderer spriteRenderer, CircleCollider2D collider2D) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
        _spriteRenderer = spriteRenderer;
        circleCollider = collider2D;
    }

    public override void Enter()
    {
        base.Enter();

        //_rigidbody.AddForce(-_enemyBase.FaceDir * 5f);

        circleCollider.enabled = false;

        originalColor = _spriteRenderer.color;
        _enemyBase.IsDead = true;
        elapsed = 0;

        _rigidbody.linearVelocity = Vector3.zero;

        if (EnemySoundManager.Instance != null)
            EnemySoundManager.Instance.SoundPlay(EnemySoundType.EnemyDead);
  
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (_triggerCalled && !_enemyBase.IsDead)
        {
            FadeOutEnemy();
        }
    }

    private void FadeOutEnemy()
    {
        elapsed += Time.deltaTime;

        float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

        if (elapsed >= fadeDuration)
        {
            _spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
            circleCollider.enabled = true;

            EnemyPoolManager.Instance.ReturnObject(_enemyBase.gameObject, _enemyBase.EnemyType);
        }
    }
}
