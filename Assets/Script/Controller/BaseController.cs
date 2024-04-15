using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody rigidBody;
    protected Animator animator;


    //���� ����
    #region StateMachine
    protected StateMachine stateMachine;
    protected Dictionary<Enum, BaseState> states = new Dictionary<Enum, BaseState>();
    #endregion


    private void Start()
    {

    }

    private void Update()
    {

    }

    //�� ��Ʈ�ѷ� ���� ������ ���� �������̵� ����
    public virtual void ChangeState(Enum state) { }
}
