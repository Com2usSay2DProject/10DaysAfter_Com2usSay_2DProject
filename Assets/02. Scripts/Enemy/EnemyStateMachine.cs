using UnityEngine;

public class EnemyStateMachine
{
    private EnemyState _currentState;
    private Enemy _enemy;

    public void InitStateMachine(EnemyState currentState, Enemy enemy)
    {
        _currentState = currentState;
        _enemy = enemy;

        Debug.Log("Init State Machine");
    }
    public void ChangeState(EnemyState state)
    {
        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }

    public void Update()
    {
        _currentState.Update();
    }
}
