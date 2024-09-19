using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : BaseController, IReceiveAttack
{
    //enum �� ���󰡰�
    //0-�Ѽհ�, 1-��հ�
    [SerializeField] List<AnimatorController> animators = new List<AnimatorController>();

    private void Awake()
    {
        Global.ChangePlayerWeaponAnim = ChangeWeaponAnimator;
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Manager.Game.Player = gameObject;
        stat = stat.StatDeepCopy();

        InitStateMachine();
        StartCoroutine(SetPlayer());
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
        //���� ����
        PlayerMoveState MoveState = new PlayerMoveState(this, rigidBody, animator);
        PlayerCrouchState CrouchState = new PlayerCrouchState(this, rigidBody, animator);
        PlayerDieState DieState = new PlayerDieState(this, rigidBody, animator);

        //���� �߰�
        states.Add(PlayerState.Move, MoveState);
        states.Add(PlayerState.Crouch, CrouchState);
        states.Add(PlayerState.Die, DieState);

        //state machine �ʱⰪ
        stateMachine = new StateMachine(MoveState);
    }

    public void ChangeWeaponAnimator(WeaponType type)
    {
        animator.runtimeAnimatorController = animators[(int)type];
    }

    public override void ChangeState(Enum state)
    {
        int s = Convert.ToInt32(state);
        stateMachine.SetState(states[(PlayerState)s]);
    }

    public void OnHit(float damage)
    {
        if (!isAlive) return;

        Debug.Log("player hitted");
        float dmg = Mathf.Max(1, damage - damage*(stat.Defense/100));
        stat.Hp -= (int)dmg;
        Global.sfx.Play(Global.Sound.hitClip, transform.position);

        if (stat.Hp <= 0)
        {
            stat.Hp = 0;

            ChangeState(PlayerState.Die);
        }
    }

    IEnumerator SetPlayer()
    {
        while(Global.PlayerSetted == null)
        {
            yield return null;
        }
        Global.PlayerSetted.Invoke(transform);
    }
}
