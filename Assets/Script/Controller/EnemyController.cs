using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : BaseController
{
    [SerializeField]
    private EnemyType enemyType;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        stat = GetComponent<Stat>();

        if (enemyType == EnemyType.Monster) InitStateMachine_Monster();
        else InitStateMachine_Human();
    }

    void Update()
    {
        stateMachine?.StateUpdateFunc();
    }

    private void FixedUpdate()
    {
        stateMachine?.StateFixtedUpdateFunc();
    }

    private void InitStateMachine_Monster()
    {
        //���� ����

        //���� �߰�

        //state machine �ʱⰪ
        //stateMachine = new StateMachine(MoveState);
    }

    private void InitStateMachine_Human()
    {
        //���� ����

        //���� �߰�

        //state machine �ʱⰪ
        //stateMachine = new StateMachine(MoveState);
    }

    public override void ChangeState(Enum state)
    {
        int s = Convert.ToInt32(state);
        stateMachine.SetState(states[(EnemyState)s]);
    }
}
