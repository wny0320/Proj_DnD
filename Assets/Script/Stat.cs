using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Stat : ScriptableObject
{
    //생성자로 아이템의 스탯을 만들 수 있게
    public Stat(int hp, int maxhp, int attack, int defense, float movespeed, float jumpforce, float attackspeed)
    {
        _hp = hp;
        _maxHp = maxhp;
        _attack = attack;
        _defense = defense;
        _moveSpeed = movespeed;
        _jumpForce = jumpforce;
        _attackSpeed = attackspeed;
    }

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

    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float AttackSpeed { get { return _attackSpeed; } set { _attackSpeed = value; } }
    public float JumpForce { get { return _jumpForce; } set { _jumpForce = value; } }
    public Stat StatDeepCopy()
    {
        Stat stat = (Stat)CreateInstance(typeof(Stat));
        stat.Hp = _hp;
        stat.MaxHp = _maxHp;
        stat.Attack = _attack;
        stat.Defense = _defense;
        stat.MoveSpeed = _moveSpeed;
        stat.JumpForce = _jumpForce;
        stat.AttackSpeed = _attackSpeed;
        return stat;
    }
}