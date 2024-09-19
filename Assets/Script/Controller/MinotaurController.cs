using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MinotaurController : BaseController, IReceiveAttack
{
    [SerializeField] EnemyWeapon weapon;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        stat = stat.StatDeepCopy();
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

    private void InitStateMachine_Monster()
    {
        //상태 생성
        MinotaurMoveState MoveState = new MinotaurMoveState(this, rigidBody, animator);
        MinotaurAttack1State AttackState1 = new MinotaurAttack1State(this, rigidBody, animator, weapon);
        MinotaurAttack2State AttackState2 = new MinotaurAttack2State(this, rigidBody, animator, weapon);
        MinotaurAttack3State AttackState3 = new MinotaurAttack3State(this, rigidBody, animator, weapon);
        EnemyDieState Diestate = new EnemyDieState(this, rigidBody, animator);

        //상태 추가
        states.Add(EnemyState.Move, MoveState);
        states.Add(EnemyState.Attack, AttackState1);
        states.Add(EnemyState.Attack2, AttackState2);
        states.Add(EnemyState.Attack3, AttackState3);
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
        if (!isAlive) return;

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

            rigidBody.isKinematic = true;
            GetComponent<Collider>().isTrigger = true;

            ChangeState(EnemyState.Die);
            states.Clear();
            stateMachine = null;
        }
    }
}
