using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CrusaderController : BaseController, IReceiveAttack
{
    [SerializeField] EnemyWeapon weapon;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //임시
        InitStat();

        InitStateMachine_Monster();
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
        stat = new Stat(200, 100, 100, 10, 10, 10, 2);
    }

    private void InitStateMachine_Monster()
    {
        //상태 생성
        CrusaderMoveState MoveState = new CrusaderMoveState(this, rigidBody, animator);
        CrusaderAttack1State Attack1State = new CrusaderAttack1State(this, rigidBody, animator, weapon);
        CrusaderAttack2State Attack2State = new CrusaderAttack2State(this, rigidBody, animator, weapon);
        CrusaderAttack3State Attack3State = new CrusaderAttack3State(this, rigidBody, animator, weapon);
        EnemyDieState Diestate = new EnemyDieState(this, rigidBody, animator);

        //상태 추가
        states.Add(EnemyState.Move, MoveState);
        states.Add(EnemyState.Attack, Attack1State);
        states.Add(EnemyState.Attack2, Attack2State);
        states.Add(EnemyState.Attack3, Attack3State);
        states.Add(EnemyState.Die, Diestate);

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
        float dmg = Mathf.Max(1, damage - stat.Defense);
        stat.Hp -= (int)dmg;
        Global.sfx.Play(Global.Sound.hitClip, transform.position);

        if (stat.Hp <= 0)
        {
            stat.Hp = 0;

            Debug.Log($"{name} die");

            GetComponent<NavMeshAgent>().SetDestination(transform.position);
            GetComponent<NavMeshAgent>().velocity = Vector3.zero;
            animator.SetBool("EnemyMove", false);

            ChangeState(EnemyState.Die);
        }
    }
}
