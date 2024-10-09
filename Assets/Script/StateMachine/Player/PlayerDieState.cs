using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
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
        animator.SetTrigger("PlayerDead");
        controller.isAlive = false;
        Manager.Game.isPlayerAlive = false;
        Debug.Log("DEAD");

        Manager.Game.OnGameEnd(false, SceneName.MainLobbyScene);
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
    }
}
