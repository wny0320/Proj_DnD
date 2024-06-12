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
    //상태 패턴
    #region StateMachine
    protected StateMachine stateMachine;
    protected Dictionary<Enum, BaseState> states = new Dictionary<Enum, BaseState>();
    #endregion

    //각 컨트롤러 상태 변경을 위해 오버라이드 ㄱㄱ
    public virtual void ChangeState(Enum state) { }
}
