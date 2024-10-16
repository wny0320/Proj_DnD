using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : BaseController, IReceiveAttack
{
    [SerializeField] List<RuntimeAnimatorController> animators = new List<RuntimeAnimatorController>();
    [SerializeField] Transform weaponTrans;
    [SerializeField] Transform leftweaponTrans;

    [SerializeField] GameObject HitEffectCanvas;
    Coroutine hitEffectCo;

    private void Awake()
    {
        Global.PlayerWeaponEquip = WeaponEquip;
        Global.PlayerWeaponUnEquip = WeaponUnequip;
        Global.PlayerArmorEquip = EquipArmor;
        Global.PlayerArmorUnEquip = UnEquipArmor;
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Manager.Game.Player = gameObject;
        stat = stat.StatDeepCopy();

        InitStateMachine();

        if(Manager.Instance.sceneName == SceneName.DungeonScene)
        {
            StartCoroutine(CheckWeaponOnStart());
            StartCoroutine(CheckArmorOnStart());
        }
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

        if (hitEffectCo != null)
        {
            StopCoroutine(hitEffectCo);
            HitSlow(false);
        }
        hitEffectCo = StartCoroutine(HitEffectCo());


        if (stat.Hp <= 0)
        {
            stat.Hp = 0;
            Global.sfx.Play(Global.Sound.PlayerDead, transform.position);
            ChangeState(PlayerState.Die);
        }
    }

    IEnumerator HitEffectCo()
    {
        HitSlow(true);
        HitEffectCanvas.SetActive(true);
        HitEffectCanvas.GetComponentInChildren<Image>().DOFade(0.25f, 0.3f).SetEase(Ease.OutElastic)
            .OnComplete(() => HitEffectCanvas.GetComponentInChildren<Image>().DOFade(0.0f, 0.2f));
        yield return new WaitForSeconds(0.5f);

        HitEffectCanvas.SetActive(false);
        HitSlow(false);
        hitEffectCo = null;
    }

    void HitSlow(bool debuff) { stat.MoveSpeed = debuff ? stat.MoveSpeed - 0.5f : stat.MoveSpeed + 0.5f; }


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
            Manager.Input.currentUtilitySlot = -1;
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
                    go.AddComponent<TwoHandedSword>();
                    break;
                case WeaponType.Consumable:
                    go.transform.localPosition = new Vector3(0.13f, 0.05f, 1);
                    go.transform.localRotation = Quaternion.Euler(180f, 0, 0);
                    go.AddComponent<Potion>().stat = stat;
                    break;
                case WeaponType.Torch:
                    go.AddComponent<TorchWeapon>();
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

        Global.PlayerWeapon = null;
    }
    #endregion

    #region 장비 장착 및 해제
    private void EquipArmor(Item armor)
    {
        if (armor.equipPart == EquipPart.Weapon) return;

        stat.MoveSpeed += armor.itemStat.MoveSpeed;
        stat.Defense += armor.itemStat.Defense;
        stat.ItemDegree += (int)armor.itemRarity;
    }

    private void UnEquipArmor(Slot slot)
    {
        if (slot.slotItem.equipPart == EquipPart.Weapon) return;

        stat.MoveSpeed -= slot.slotItem.itemStat.MoveSpeed;
        stat.Defense -= slot.slotItem.itemStat.Defense;
        stat.ItemDegree -= (int)slot.slotItem.itemRarity;
    }

    IEnumerator CheckArmorOnStart()
    {
        foreach(var part in Enum.GetValues(typeof(EquipPart)))
        {
            while (!Manager.Inven.equipSlots.ContainsKey(part.ToString()))
            {
                yield return null;
            }

            Item armor = Manager.Inven.equipSlots[part.ToString()].slotItem;
            if (armor == null)
            {
                //Debug.Log($"{part} None");
                continue;
            }
            //Debug.Log($"{part} equip");
            EquipArmor(armor);
        }
    }
    #endregion
}
