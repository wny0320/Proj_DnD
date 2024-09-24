using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : BaseController, IReceiveAttack
{
    [SerializeField] List<AnimatorController> animators = new List<AnimatorController>();
    [SerializeField] Transform weaponTrans;
    [SerializeField] Transform leftweaponTrans;

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
        StartCoroutine(CheckWeaponOnStart());
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

    #region ���� ���� �� ����
    IEnumerator CheckWeaponOnStart()
    {
        while(!Manager.Inven.equipSlots.ContainsKey(EquipPart.Weapon.ToString() + 1))
        {
            yield return null;
        }
        Item weapon = Manager.Inven.equipSlots[EquipPart.Weapon.ToString() + 1].slotItem;
        
        WeaponEquip(weapon);
    }

    private void WeaponEquip(Item equipWeapon)
    {
        //���� �� ���� ����
        if(Global.PlayerWeapon != null)
        {
            if (Global.PlayerWeapon.GetComponent<Item3D>() == null) WeaponUnequip(null);
            else WeaponUnequip(Global.PlayerWeapon.GetComponent<Item3D>().myItem);
        }

        //�Ǽ�
        if (equipWeapon == null)
        {
            weaponTrans.GetChild(0).gameObject.SetActive(true);
            ChangeWeaponAnimator(WeaponType.BareHand);
        }
        else
        {
            weaponTrans.GetChild(0).gameObject.SetActive(false);

            //���Ⱥ���
            stat.MoveSpeed += equipWeapon.itemStat.MoveSpeed;
            //�ִ� ����
            ChangeWeaponAnimator(equipWeapon.weaponType);
            //���� ����
            GameObject go = Instantiate(Manager.Data.item3DPrefab[equipWeapon.itemIndex], weaponTrans);
            
            switch(equipWeapon.weaponType)
            {
                case WeaponType.Onehanded:
                    go.AddComponent<Sword>();
                    break;
                case WeaponType.Twohanded:
                    go.AddComponent<Sword>();
                    break;
                case WeaponType.Consumable:
                    go.transform.localPosition = new Vector3(0.13f, 0.05f, 1);
                    go.transform.localRotation = Quaternion.Euler(180f, 0, 0);
                    go.AddComponent<Potion>();
                    break;
            }
        }
    }

    private void WeaponUnequip(Item equipWeapon)
    {
        if (equipWeapon == null)
        {
            return;
        }

        stat.MoveSpeed -= equipWeapon.itemStat.MoveSpeed;
        Destroy(Global.PlayerWeapon.gameObject);
        //���ָ����� �����ϴ� �ִ� �־�ߵ�

        Global.PlayerWeapon = null;
    }
    #endregion


}
