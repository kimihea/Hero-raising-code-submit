using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState 
{
    public void Enter();
    public void Exit();
    public void Update();
}
public class StateMachine
{
    public IState currentState;
    public IState previousState;
    public void ChangeState(IState state)
    {
        currentState?.Exit();
        previousState = currentState;
        currentState = state;
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();

    }

}
