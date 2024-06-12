using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stat))]
public class BaseController : MonoBehaviour
{
    protected Rigidbody rigidBody;
    protected Animator animator;

    public Stat stat;

    public bool isAlive = true;
    //���� ����
    #region StateMachine
    protected StateMachine stateMachine;
    protected Dictionary<Enum, BaseState> states = new Dictionary<Enum, BaseState>();
    #endregion

    //�� ��Ʈ�ѷ� ���� ������ ���� �������̵� ����
    public virtual void ChangeState(Enum state) { }
}
