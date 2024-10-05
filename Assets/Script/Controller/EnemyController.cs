using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseController, IReceiveAttack
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
        EnemyMoveState MoveState = new EnemyMoveState(this, rigidBody, animator);
        EnemyAttackState AttackState = new EnemyAttackState(this, rigidBody, animator, weapon);
        EnemyDieState Diestate = new EnemyDieState(this, rigidBody, animator);

        //상태 추가
        states.Add(EnemyState.Move, MoveState);
        states.Add(EnemyState.Attack, AttackState);
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
            OnDead();
        }
    }

    private void OnDead()
    {
        Global.sfx.Play(Global.Sound.SkeletonDead, transform.position);

        weapon.GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().SetDestination(transform.position);
        GetComponent<NavMeshAgent>().velocity = Vector3.zero;
        animator.SetBool("EnemyMove", false);

        //rigidBody.isKinematic = true;
        //GetComponent<Collider>().isTrigger = true;

        ChangeState(EnemyState.Die);
        states.Clear();
        stateMachine = null;

        animator.enabled = false;

        RagdollDead(transform);
        gameObject.AddComponent<Interactive>();
        Destroy(gameObject.GetComponent<BaseController>());
    }

    private void RagdollDead(Transform trans)
    {
        trans.gameObject.layer = LayerMask.NameToLayer("Interactive");
        Rigidbody r = trans.GetComponent<Rigidbody>();
        Collider c = trans.GetComponent<Collider>();
        if (r != null)
            r.isKinematic = false;
        if (c != null)
            c.excludeLayers = 1 << LayerMask.NameToLayer("Player") |
                1 << LayerMask.NameToLayer("Monster") | 1 << LayerMask.NameToLayer("Traveler");

        foreach (Transform child in trans)
        {
            RagdollDead(child);
        }
    }
}
