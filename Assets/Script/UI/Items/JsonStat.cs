using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonStat
{
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected float _moveSpeed;
    [SerializeField]
    protected float _jumpForce;

    [SerializeField]
    protected float _attackSpeed;
    [SerializeField]
    protected int _itemDegree; //아이템 등급

    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float AttackSpeed { get { return _attackSpeed; } set { _attackSpeed = value; } }
    public float JumpForce { get { return _jumpForce; } set { _jumpForce = value; } }
    public int ItemDegree { get { return _itemDegree; } set { _itemDegree = value; } }

    public Stat JsonStatToStat()
    {
        Stat stat = (Stat)ScriptableObject.CreateInstance(typeof(Stat));
        stat.Hp = Hp;
        stat.MaxHp = MaxHp;
        stat.Attack = Attack;
        stat.Defense =Defense;
        stat.MoveSpeed = MoveSpeed;
        stat.JumpForce = JumpForce;
        stat.AttackSpeed = AttackSpeed;
        stat.ItemDegree = ItemDegree;
        return stat;
    }
}
