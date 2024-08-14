using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WatcherController : BaseController, IReceiveAttack
{
    [SerializeField] EnemyWeapon weapon;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        //�ӽ�
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
        stat = new Stat(200, 100, 100, 10, 1, 10, 2);
    }

    private void InitStateMachine_Monster()
    {
        //���� ����
        WatcherMoveState MoveState = new WatcherMoveState(this, rigidBody, animator);
        WatcherAttackState AttackState = new WatcherAttackState(this, rigidBody, animator, weapon);
        EnemyDieState Diestate = new EnemyDieState(this, rigidBody, animator);

        //���� �߰�
        states.Add(EnemyState.Move, MoveState);
        states.Add(EnemyState.Attack, AttackState);
        states.Add(EnemyState.Die, Diestate);

        //state machine �ʱⰪ
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
        if (stat.Hp <= 0)
        {
            stat.Hp = 0;
            rigidBody.velocity = Vector3.zero;

            Debug.Log($"{name} die");

            animator.SetBool("EnemyMove", false);

            ChangeState(EnemyState.Die);
        }
    }
}
