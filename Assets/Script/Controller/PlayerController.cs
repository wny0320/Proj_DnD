using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : BaseController, IReceiveAttack
{
    private void Awake()
    {
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Manager.Game.Player = gameObject;

        //initStat 임시
        InitStat();

        InitStateMachine();
    }

    void Update()
    {
        stateMachine?.StateUpdateFunc();
    }

    private void FixedUpdate()
    {
        stateMachine?.StateFixtedUpdateFunc();
    }

    private void InitStat()
    {
        stat = new Stat(100, 100, 100, 10, 10, 10, 0);
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
        states.Add(PlayerState.Die, DieState);

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
        float dmg = Mathf.Max(1, damage - stat.Defense);
        stat.Hp -= (int)dmg;
        if (stat.Hp <= 0)
        {
            stat.Hp = 0;

            ChangeState(PlayerState.Die);
        }
    }

}
