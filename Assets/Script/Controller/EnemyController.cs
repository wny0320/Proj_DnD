using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : BaseController, IReceiveAttack
{
    [SerializeField]
    private EnemyType enemyType;

    public List<Vector3> wayPoints = new();

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
        //상태 생성
        EnemyMoveState MoveState = new EnemyMoveState(this, rigidBody, animator);
        EnemyAttackState AttackState = new EnemyAttackState(this, rigidBody, animator);

        //상태 추가
        states.Add(EnemyState.Move, MoveState);
        states.Add(EnemyState.Attack, AttackState);

        //state machine 초기값
        stateMachine = new StateMachine(MoveState);
    }

    private void InitStateMachine_Human()
    {
        //상태 생성
        EnemyMoveState MoveState = new EnemyMoveState(this, rigidBody, animator);
        EnemyAttackState AttackState = new EnemyAttackState(this, rigidBody, animator);

        //상태 추가
        states.Add(EnemyState.Move, MoveState);
        states.Add(EnemyState.Attack, AttackState);

        //state machine 초기값
        stateMachine = new StateMachine(MoveState);
    }

    public override void ChangeState(Enum state)
    {
        int s = Convert.ToInt32(state);
        stateMachine.SetState(states[(EnemyState)s]);
    }

    public void OnHit(float damage)
    {
        //피격함수
    }
}
