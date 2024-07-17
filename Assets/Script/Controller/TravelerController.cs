using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TravelerController : BaseController, IReceiveAttack
{
    [SerializeField] private EnemyWeapon weapon;

    private Coroutine AttackCo = null;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //�ӽ�
        InitStat();

        InitStateMachine_Human();
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
        stat = new Stat(100, 100, 100, 10, 10, 10, 5);
    }

    private void InitStateMachine_Human()
    {
        //���� ����
        TravelerMoveState MoveState = new TravelerMoveState(this, rigidBody, animator);
        EnemyAttackState AttackState = new EnemyAttackState(this, rigidBody, animator, weapon);
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

        if ((EnemyState)s == EnemyState.Attack && AttackCo == null)
        {
            AttackCo = StartCoroutine(AttackCoroutine());
        }
    }

    public void OnHit(float damage)
    {
        float dmg = Mathf.Max(1, damage - stat.Defense);
        stat.Hp -= (int)dmg;
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

    IEnumerator AttackCoroutine()
    {
        weapon.AttackStart();

        //�̺κ� ��� ���� �ִϸ��̼� ���� �ð����� �����ؾߵ�
        yield return new WaitForSeconds(1.5f);

        weapon.AttackEnd();
        AttackCo = null;
    }
}
