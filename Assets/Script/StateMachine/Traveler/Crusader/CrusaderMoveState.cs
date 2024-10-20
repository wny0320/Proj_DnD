using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CrusaderMoveState : BaseState
{
    const string ENEMY_MOVE = "EnemyMove";
    const string ENEMY_ATTACK = "EnemyAttack";
    const string ENEMY_RUN = "EnemyRun";

    //적 추적 및 이동
    private bool isFind = false;
    private float travelingDistance = 30f;
    private float chaseDistance = 60f; //플레이어 추격 거리
    private float forwardDetectRange = 20f; //전방 감지 거리
    private float senseDetectRange = 7f; //주변 감지 거리
    private float attackDistance = 1.25f;

    private float attackSpeed;
    private float attackCooldown = 0f;
    private bool canAttack = true;
    private bool isAttacking = false;

    private NavMeshAgent agent;
    private Transform target;
    private Transform transform;
        
    private Vector3 originPos;
    private Vector3 randomPos = Vector3.zero;
    private bool isArrived = false;
    private float lookAroundTime = 0f;

    private Vector3 fleePos= Vector3.zero;
    private float fleeDistance = 10f;
    private bool isFleeing = false;

    private bool isBranchSetted = false;
    private int branchInt = 0;

    NavMeshPath path = new NavMeshPath();

    public CrusaderMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        agent = controller.GetComponent<NavMeshAgent>();
        agent.speed = controller.stat.MoveSpeed;

        transform = controller.transform;
        originPos = transform.position;

        //공격 주기
        attackSpeed = controller.stat.AttackSpeed;
    }

    public override void OnFixedUpdate()
    {
        attackCooldown += Time.fixedDeltaTime;

        if (attackCooldown > attackSpeed) { canAttack = true; attackCooldown = 0f; }
    }

    public override void OnStateEnter()
    {
        isAttacking = false;
        if ((agent.destination - transform.position).magnitude <= agent.radius * 2)
        {
            isArrived = true;
            randomPos = Vector3.zero;
        }
        else isArrived = false;

        if (!DetectPlayer(10f)) isFind = false;

        attackCooldown = 0f;
    }

    public override void OnStateExit()
    {
        isAttacking = false;
        isArrived = false;
    }

    public override void OnStateUpdate()
    {
        //CheckStuck();

        if (!isFind)
        {
            Traveling();
            DetectPlayer(senseDetectRange);
        }
        else
        {
            if (target.GetComponent<BaseController>() == null)
            {
                isFind = false;
                return;
            }
            //조건
            if (BranchChaseFlee())
                Chase();
            else
                Flee();
        }

        if (Mathf.Abs(agent.velocity.x) > 0.2f || Mathf.Abs(agent.velocity.z) > 0.2f)
        {
            if (isFleeing) { animator.SetBool(ENEMY_RUN, true); animator.SetBool(ENEMY_MOVE, false); }
            else { animator.SetBool(ENEMY_MOVE, true); animator.SetBool(ENEMY_RUN, false); }
        }
        else
        {
            animator.SetBool(ENEMY_MOVE, false);
            animator.SetBool(ENEMY_RUN, false);
        }
    }

    private void Traveling()
    {
        if(randomPos == Vector3.zero)
        {
            if(isArrived)
            {
                lookAroundTime += Time.deltaTime;
                if(lookAroundTime >= 2.5f)
                {
                    isArrived = false;
                    lookAroundTime = 0f;
                }
                return;
            }

            Vector3 rand = Random.insideUnitSphere * travelingDistance;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(rand + transform.position, out hit, travelingDistance, NavMesh.AllAreas))
            {
                if (agent.CalculatePath(hit.position, path) && path.status != NavMeshPathStatus.PathPartial && path.status != NavMeshPathStatus.PathInvalid)
                {
                    randomPos = hit.position;
                    agent.SetDestination(randomPos);
                }
                else
                {
                    randomPos = Vector3.zero;
                }
            }
            else
            {
                randomPos = Vector3.zero;
            }
        }

        if ((transform.position - randomPos).magnitude <= 1 + agent.radius*2)
        {
            originPos= transform.position;
            randomPos = Vector3.zero;
            isArrived= true;
            if(Random.Range(0,10) > 5)
                animator.Play("LookAround");
        }

    }

    private void Flee()
    {
        isFleeing= true;

        if (fleePos == Vector3.zero)
        {
            Vector3 dir = (transform.position - target.position).normalized;

            Vector3 rand = (Random.insideUnitSphere + dir) * fleeDistance;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(rand + transform.position, out hit, fleeDistance, NavMesh.AllAreas))
            {
                if (agent.CalculatePath(hit.position, path) && path.status != NavMeshPathStatus.PathPartial && path.status != NavMeshPathStatus.PathInvalid)
                {
                    fleePos = hit.position;
                    agent.SetDestination(fleePos);
                }
                else
                    fleePos = Vector3.zero;
            }
            else
                fleePos = Vector3.zero;
        }

        if ((transform.position - fleePos).magnitude <= agent.radius * 2)
        {
            fleePos = Vector3.zero;
            isFleeing = false;
            isFind = false;
            randomPos = Vector3.zero;

            isBranchSetted = false;
        }
    }


    private bool DetectPlayer(float DetectDistance)
    {
        float distance = 999f;
        Transform t = null;
        //주변 탐지
        Collider[] cols = Physics.OverlapSphere(transform.position, DetectDistance, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Monster"));
        foreach (Collider col in cols)
        {
            BaseController bc = col.GetComponent<BaseController>();
            if (bc == null || !bc.isAlive) continue;

            if (Vector3.Magnitude(col.transform.position - transform.position) < distance)
            {
                distance = Vector3.Magnitude(col.transform.position - transform.position);
                originPos = transform.position;

                t = col.transform;
                isFind = true;
            }
        }
        if (t != null)
        {
            target = t;
            return true;
        }

        // 전방 부채꼴 탐지
        Collider[] cols2 = Physics.OverlapSphere(transform.position, forwardDetectRange, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Monster"));
        foreach (Collider col in cols2)
        {
            BaseController bc = col.GetComponent<BaseController>();
            if (bc == null || !bc.isAlive) continue;

            Vector3 dist = col.transform.position - transform.position;
            if (dist.magnitude <= forwardDetectRange)
            {
                float dot = Vector3.Dot(dist.normalized, transform.forward);
                float theta = Mathf.Acos(dot);
                float degree = Mathf.Rad2Deg * theta;

                if (degree <= 80f)
                {
                    RaycastHit hit;
                    Physics.Raycast(transform.position, dist.normalized, out hit, forwardDetectRange + 3);

                    if (hit.collider == col)
                    {
                        if (Vector3.Magnitude(col.transform.position - transform.position) < distance)
                        {
                            distance = Vector3.Magnitude(col.transform.position - transform.position);
                            originPos = transform.position;

                            t = col.transform;
                            isFind = true;
                        }
                    }
                }
            }
        }

        if (t != null)
        {
            target = t;
            return true;
        }

        return false;
    }


    private void Chase()
    {
        float distance = CheckDistance();
        float subAtkDistance = 0f;
        if (target != null && target.GetComponent<MinotaurController>() != null)
            subAtkDistance = 1f;
        if (distance <= attackDistance + agent.radius + subAtkDistance)
        {
            if (!isAttacking)
            {
                Attack();
            }
            isBranchSetted = false;
            return;
        }

        float moveDist = (originPos - transform.position).magnitude;
        if (moveDist > chaseDistance)
        {
            if (distance < senseDetectRange)
            {
                if (agent.CalculatePath(target.position, path) && path.status != NavMeshPathStatus.PathPartial && path.status != NavMeshPathStatus.PathInvalid)
                {
                    agent.SetDestination(target.position);
                }
            }
            else
            {
                agent.SetDestination(originPos);
                isFind = false;
                isBranchSetted = false;
                return;
            }
        }
        else if (distance <= chaseDistance)
        {
            if (agent.CalculatePath(target.position, path) && path.status != NavMeshPathStatus.PathPartial && path.status != NavMeshPathStatus.PathInvalid)
            {
                agent.SetDestination(target.position);
            }
        }
        else
        {
            isFind = false;
            isBranchSetted = false;
        }
    }

    private void Attack()
    {
        agent.SetDestination(transform.position); //멈추기
        agent.velocity = Vector3.zero;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        if (!target.GetComponent<BaseController>().isAlive) return;

        if (canAttack)
        {
            animator.SetTrigger(ENEMY_ATTACK);
            animator.SetBool(ENEMY_MOVE, false);
            animator.SetBool(ENEMY_RUN, false);
            canAttack = false;
            isAttacking = true;
            controller.ChangeState(EnemyState.Attack);
            return;
        }
    }

    private float CheckDistance()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target.position, path);

        //경로 거리 계산을 위한 벡터
        Vector3[] wayPoint = new Vector3[path.corners.Length + 2];
        wayPoint[0] = transform.position;
        wayPoint[wayPoint.Length - 1] = target.position;

        float distance = 0f; //거리
        for (int p = 0; p < path.corners.Length; p++)
        {
            wayPoint[p + 1] = path.corners[p];
            distance += Vector3.Distance(wayPoint[p], wayPoint[p + 1]);
        }

        return distance;
    }

    private bool BranchChaseFlee()
    {
        //true - chase, false - flee
        if (target == null) return false;

        if(!isBranchSetted)
        {
            isBranchSetted = true;
            branchInt = (int)Random.Range(20, 21 + (float)controller.stat.Hp / (float)controller.stat.MaxHp * 50);
        }

        if (target.GetComponent<BaseController>().stat.ItemDegree > branchInt)
            return false;

        return true;
    }
}
