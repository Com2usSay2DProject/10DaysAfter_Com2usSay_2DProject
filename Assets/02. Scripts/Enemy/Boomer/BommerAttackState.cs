using UnityEngine;

public class BommerAttackState : EnemyAttackState
{
    SpriteRenderer _spriteRenderer;
    private float fadeDuration = 1f;
    private float elapsed;
    private Color originalColor;

    public BommerAttackState(EnemyStateMachine stateMachine, Rigidbody2D rigidbody2D, Enemy enemy, SpriteRenderer spriteRenderer, string animBoolName) : base(stateMachine, rigidbody2D, enemy, animBoolName)
    {
        _spriteRenderer = spriteRenderer;
    }

    public override void Enter()
    {
        base.Enter();
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
            FadeOutEnemy();
        }
    }

    private void FadeOutEnemy()
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
