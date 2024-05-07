using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public BaseState CurrentState { get; private set; }

    public StateMachine(BaseState defaultState) => CurrentState = defaultState;

    //state ½ºÀ§Äª ÇÔ¼ö
    public void SetState(BaseState state)
    {
        if (CurrentState == state) { Debug.Log("SameState"); return; }

        CurrentState.OnStateExit();
        CurrentState = state;
        CurrentState.OnStateEnter();
    }

    //state update
    public void StateUpdateFunc()
    {
        CurrentState.OnStateUpdate();
        Debug.Log(CurrentState);
    }

    public void StateFixtedUpdateFunc()
    {
        CurrentState.OnFixedUpdate();
    }
}
