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
        if (!isAlive) return;

        float dmg = Mathf.Max(1, damage - stat.Defense);
        stat.Hp -= (int)dmg;
        Global.sfx.Play(Global.Sound.hitClip, transform.position);

        if (stat.Hp <= 0)
        {
            stat.Hp = 0;
            OnDead();
        }
    }

    private void OnDead()
    {
        ChangeLayer(transform);
        Global.sfx.Play(Global.Sound.WatcherDead, transform.position);

        transform.rotation = Quaternion.Euler(Vector3.zero);

        animator.SetBool("EnemyMove", false);

        rigidBody.isKinematic = true;
        GetComponentInChildren<Collider>().isTrigger = true;

        ChangeState(EnemyState.Die);
        states.Clear();
        stateMachine = null;

        gameObject.AddComponent<Interactive>();
        Destroy(gameObject.GetComponent<BaseController>());
    }

    private void ChangeLayer(Transform trans)
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
            ChangeLayer(child);
        }
    }
}
