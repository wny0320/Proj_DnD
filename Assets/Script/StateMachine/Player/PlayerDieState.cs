using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : BaseState
{
    public PlayerDieState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {

    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStateEnter()
    {
        Debug.Log("player die");
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
    }
}
