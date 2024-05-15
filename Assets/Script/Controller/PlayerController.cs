using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : BaseController, IReceiveAttack
{
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        stat = GetComponent<Stat>();

        InitStateMachine();

        Manager.Game.Player = gameObject;
        
    }

    void Update()
    {
        stateMachine?.StateUpdateFunc();
    }

    private void FixedUpdate()
    {
        stateMachine?.StateFixtedUpdateFunc();
    }

    private void InitStateMachine()
    {
        //상태 생성
        PlayerMoveState MoveState = new PlayerMoveState(this, rigidBody, animator);
        PlayerCrouchState CrouchState = new PlayerCrouchState(this, rigidBody, animator);
        PlayerDieState DieState = new PlayerDieState(this, rigidBody, animator);

        //상태 추가
        states.Add(PlayerState.Move, MoveState);
        states.Add(PlayerState.Crouch, CrouchState);

        //state machine 초기값
        stateMachine = new StateMachine(MoveState);
    }

    public override void ChangeState(Enum state)
    {
        int s = Convert.ToInt32(state);
        stateMachine.SetState(states[(PlayerState)s]);
    }

    public void OnHit(float damage)
    {
        //피격 함수
    }
}
