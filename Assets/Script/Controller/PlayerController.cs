using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerController : BaseController, IReceiveAttack
{
    //enum 값 따라가게
    //0-한손검, 1-양손검
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
        //상태 생성
        PlayerMoveState MoveState = new PlayerMoveState(this, rigidBody, animator);
        PlayerCrouchState CrouchState = new PlayerCrouchState(this, rigidBody, animator);
        PlayerDieState DieState = new PlayerDieState(this, rigidBody, animator);

        //상태 추가
        states.Add(PlayerState.Move, MoveState);
        states.Add(PlayerState.Crouch, CrouchState);
        states.Add(PlayerState.Die, DieState);

        //state machine 초기값
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
        //장착 전 장착 해제
        if(Global.PlayerWeapon != null)
            WeaponUnequip(Global.PlayerWeapon.GetComponent<Item3D>().myItem);

        //스탯변경
        stat.MoveSpeed = stat.MoveSpeed += equipWeapon.itemStat.MoveSpeed;
        //애니 변경
        ChangeWeaponAnimator(equipWeapon.weaponType);
        //무기 적용
        Instantiate(Manager.Data.item3DPrefab[equipWeapon.itemIndex], weaponTrans);
    }

    private void WeaponUnequip(Item equipWeapon)
    {
        stat.MoveSpeed = stat.MoveSpeed -= equipWeapon.itemStat.MoveSpeed;
        
        //맨주먹으로 변경하는 애니 넣어야됨

        Global.PlayerWeapon = null;
    }
}
