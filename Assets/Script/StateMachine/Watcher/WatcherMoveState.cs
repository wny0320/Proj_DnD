using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WatcherMoveState : BaseState
{
    const string ENEMY_MOVE = "EnemyMove";
    const string ENEMY_ATTACK = "EnemyAttack";

    //적 추적 및 이동
    private bool isFind = false;
    private float chaseDistance = 20f; //플레이어 추격 거리
    private float senseDetectRange = 15f; //주변 감지 거리
    private float attackDistance = 1f;
    private float moveSpeed;

    private float attackSpeed;
    private float attackCooldown = 0f;
    private bool canAttack = true;
    private bool isAttacking = false;

    private Transform target;
    private Transform transform;

    private Vector3 originPos;

    public WatcherMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        transform = controller.transform;
        originPos = transform.position;
        moveSpeed = controller.stat.MoveSpeed;
        //공격 주기
        attackSpeed = controller.stat.AttackSpeed;


        Global.PlayerSetted -= GetPlayer;
        Global.PlayerSetted += GetPlayer;
    }

    public override void OnFixedUpdate()
    {
        attackCooldown += Time.fixedDeltaTime;

        if (attackCooldown > attackSpeed) { canAttack = true; attackCooldown = 0f; }
    }

    public override void OnStateEnter()
    {
        isAttacking = false;

        if (!DetectPlayer(10f)) isFind = false;

        attackCooldown = 0f;
    }

    public override void OnStateExit()
    {
        isAttacking = false;
    }

    public override void OnStateUpdate()
    {
        if (!isFind)
        {
            if ((transform.position - originPos).magnitude < 1f)
                rb.velocity = Vector3.zero;
            DetectPlayer(senseDetectRange);
        }
        else Chase();

        if (Mathf.Abs(rb.velocity.x) > 0.2f || Mathf.Abs(rb.velocity.z) > 0.2f) animator.SetBool(ENEMY_MOVE, true);
        else animator.SetBool(ENEMY_MOVE, false);
    }

    private bool DetectPlayer(float DetectDistance)
    {
        //if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null) return false;

        //주변 탐지
        Collider[] cols = Physics.OverlapSphere(transform.position, DetectDistance, 1 << 10);
        foreach (Collider col in cols)
        {
            if (col.gameObject == target.gameObject)
            {
                isFind = true;
                return true;
            }
        }

        return false;
    }

    private void Chase()
    {
        float distance = (transform.position - target.position).magnitude; 
        if (distance <= attackDistance)
        {
            animator.SetBool(ENEMY_ATTACK, true);
            transform.LookAt(target.position);
            if (!isAttacking)
            {
                Attack();
            }

            return;
        }

        float moveDist = (originPos - transform.position).magnitude;
        if (moveDist > chaseDistance)
        {
            if (distance < senseDetectRange)
            {
                transform.LookAt(target);
                rb.velocity = (target.position - transform.position) * moveSpeed;
            }
            else
            {
                animator.SetBool(ENEMY_ATTACK, false);

                transform.LookAt(originPos);
                rb.velocity = (originPos - transform.position) * moveSpeed;
                isFind = false;
                return;
            }
        }
        else if (distance <= chaseDistance)
        {
            transform.LookAt(target);
            rb.velocity = (target.position - transform.position) * moveSpeed;
        }
        else
            isFind = false;

    }

    private void Attack()
    {
        rb.velocity = Vector3.zero;
        transform.LookAt(target);

        if (!Manager.Game.isPlayerAlive) return;

        if (canAttack)
        {
            animator.SetBool(ENEMY_ATTACK, true);
            animator.SetBool(ENEMY_MOVE, false);
            canAttack = false;
            isAttacking = true;
            controller.ChangeState(EnemyState.Attack);
            return;
        }
    }

    private void GetPlayer(Transform player) => target = player;
}
