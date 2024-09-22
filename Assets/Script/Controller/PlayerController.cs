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
    [SerializeField] Transform weaponTrans;

    private void Awake()
    {
        Global.PlayerWeaponEquip -= WeaponEquip;
        Global.PlayerWeaponUnEquip -= WeaponUnequip;

        Global.PlayerWeaponEquip += WeaponEquip;
        Global.PlayerWeaponUnEquip += WeaponUnequip;
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Manager.Game.Player = gameObject;
        stat = stat.StatDeepCopy();

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

    private void WeaponEquip(Item equipWeapon)
    {
        //���� �� ���� ����
        if(Global.PlayerWeapon != null)
            WeaponUnequip(Global.PlayerWeapon.GetComponent<Item3D>().myItem);

        //���Ⱥ���
        stat.MoveSpeed = stat.MoveSpeed += equipWeapon.itemStat.MoveSpeed;
        //�ִ� ����
        ChangeWeaponAnimator(equipWeapon.weaponType);
        //���� ����
        Instantiate(Manager.Data.item3DPrefab[equipWeapon.itemIndex], weaponTrans);
    }

    private void WeaponUnequip(Item equipWeapon)
    {
        stat.MoveSpeed = stat.MoveSpeed -= equipWeapon.itemStat.MoveSpeed;
        
        //���ָ����� �����ϴ� �ִ� �־�ߵ�

        Global.PlayerWeapon = null;
    }
}
