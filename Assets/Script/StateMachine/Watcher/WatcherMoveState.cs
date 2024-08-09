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
    //private float forwardDetectRange = 15f; //전방 감지 거리
    private float senseDetectRange = 5f; //주변 감지 거리
    private float attackDistance = 2f;

    private float attackSpeed;
    private float attackCooldown = 0f;
    private bool canAttack = true;
    private bool isAttacking = false;

    private Transform target;
    private Transform transform;

    private Vector3 originPos;
    private List<Vector3> wayPoints = new List<Vector3>();
    private int idx = 0;

    public WatcherMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        transform = controller.transform;
        originPos = transform.position;

        //공격 주기
        attackSpeed = controller.stat.AttackSpeed;

        //패트롤 웨이 포인트
        Transform wp = transform.Find("Waypoints");
        foreach (Transform t in wp)
            wayPoints.Add(t.position);

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
            DetectPlayer(senseDetectRange);
        }
        else Chase();

        //if (Mathf.Abs(agent.velocity.x) > 0.2f || Mathf.Abs(agent.velocity.z) > 0.2f) animator.SetBool(ENEMY_MOVE, true);
        //else animator.SetBool(ENEMY_MOVE, false);
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
        //float distance = CheckDistance();
        //if (distance <= attackDistance + agent.radius)
        //{
        //    if (!isAttacking)
        //    {
        //        Attack();
        //    }

        //    return;
        //}

        //float moveDist = (originPos - transform.position).magnitude;
        //if (moveDist > chaseDistance)
        //{
        //    if (distance < senseDetectRange)
        //        agent.SetDestination(target.position);
        //    else
        //    {
        //        agent.SetDestination(originPos);
        //        isFind = false;
        //        return;
        //    }
        //}
        //else if (distance <= chaseDistance)
        //    agent.SetDestination(target.position);
        //else
        //    isFind = false;

    }

    private void Attack()
    {
        //agent.SetDestination(transform.position); //멈추기
        //agent.velocity = Vector3.zero;
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        if (!Manager.Game.isPlayerAlive) return;

        if (canAttack)
        {
            animator.SetTrigger(ENEMY_ATTACK);
            animator.SetBool(ENEMY_MOVE, false);
            canAttack = false;
            isAttacking = true;
            controller.ChangeState(EnemyState.Attack);
            return;
        }
    }

    //private float CheckDistance()
    //{
    //    NavMeshPath path = new NavMeshPath();
    //    agent.CalculatePath(target.position, path);

    //    //경로 거리 계산을 위한 벡터
    //    Vector3[] wayPoint = new Vector3[path.corners.Length + 2];
    //    wayPoint[0] = transform.position;
    //    wayPoint[wayPoint.Length - 1] = target.position;

    //    float distance = 0f; //거리
    //    for (int p = 0; p < path.corners.Length; p++)
    //    {
    //        wayPoint[p + 1] = path.corners[p];
    //        distance += Vector3.Distance(wayPoint[p], wayPoint[p + 1]);
    //    }

    //    return distance;
    //}

    private void GetPlayer(Transform player) => target = player;
}
