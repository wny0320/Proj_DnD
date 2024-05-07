using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
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

    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public float AttackSpeed { get { return _attackSpeed; } set { _attackSpeed = value; } }
    public float JumpForce { get { return _jumpForce; } set { _jumpForce = value; } }


    private void Awake()
    {
        Init();
    }

    //�ǰ��� �������̽��� �Ұ��� ���� �Լ��� �Ұ��� ���ؾ� �ɵ�
    public virtual void OnAttacked(Stat attacker)
    {
        int damage = Mathf.Max(0, attacker.Attack - Defense);
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            //Ondead - ���ó��
        }
    }

    private void Init()
    {
        //���� �ʱⰪ
    }
}