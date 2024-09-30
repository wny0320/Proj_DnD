using DG.Tweening;
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
        Global.PlayerArmorEquip -= EquipArmor;
        Global.PlayerArmorUnEquip -= UnEquipArmor;

        Global.PlayerWeaponEquip += WeaponEquip;
        Global.PlayerWeaponUnEquip += WeaponUnequip;
        Global.PlayerArmorEquip += EquipArmor;
        Global.PlayerArmorUnEquip += UnEquipArmor;
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

    #region 무기 장착 및 해제
    IEnumerator CheckWeaponOnStart()
    {
        while(!Manager.Inven.equipSlots.ContainsKey(EquipPart.Weapon.ToString() + 0))
        {
            yield return null;
        }
        Item weapon = Manager.Inven.equipSlots[EquipPart.Weapon.ToString() + 0].slotItem;
        
        WeaponEquip(weapon);
    }

    private void WeaponEquip(Item equipWeapon)
    {
        //장착 전 장착 해제
        if(Global.PlayerWeapon != null)
        {
            if (Global.PlayerWeapon.GetComponent<Item3D>() == null) WeaponUnequip(null);
            else WeaponUnequip(Global.PlayerWeapon.GetComponent<Item3D>().myItem);
        }

        //맨손
        if (equipWeapon == null)
        {
            Manager.Input.currentWeaponSlot = -1;
            weaponTrans.GetChild(0).gameObject.SetActive(true);
            ChangeWeaponAnimator(WeaponType.BareHand);
        }
        else
        {
            weaponTrans.GetChild(0).gameObject.SetActive(false);

            //스탯변경
            stat.MoveSpeed += equipWeapon.itemStat.MoveSpeed;
            //애니 변경, 같은 타입의 무기 변경 시 맨손 거쳐가야됨
            ChangeWeaponAnimator(WeaponType.BareHand);
            ChangeWeaponAnimator(equipWeapon.weaponType);
            //무기 적용
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
                    go.AddComponent<Potion>().stat = stat;
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
        //맨주먹으로 변경하는 애니 넣어야됨

        Global.PlayerWeapon = null;
    }
    #endregion

    #region 장비 장착 및 해제
    private void EquipArmor(Item armor)
    {
        stat.MoveSpeed += armor.itemStat.MoveSpeed;
        stat.Defense += armor.itemStat.Defense;
    }

    private void UnEquipArmor(Slot slot)
    {
        stat.MoveSpeed -= slot.slotItem.itemStat.MoveSpeed;
        stat.Defense -= slot.slotItem.itemStat.Defense;
    }
    #endregion
}
