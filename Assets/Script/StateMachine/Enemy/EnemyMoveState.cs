using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveState : BaseState
{
    const string ENEMY_MOVE = "EnemyMove";
    const string ENEMY_ATTACK = "EnemyAttack";

    //�� ���� �� �̵�
    private bool isFind = false;
    private float chaseDistance = 20f; //�÷��̾� �߰� �Ÿ�
    private float forwardDetectRange = 15f; //���� ���� �Ÿ�
    private float senseDetectRange = 5f; //�ֺ� ���� �Ÿ�
    private float attackDistance = 2f;

    private float attackSpeed;
    private float attackCooldown = 0f;
    private bool canAttack = true;

    private NavMeshAgent agent;
    private Transform target;
    private Transform transform;

    public EnemyMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        agent = controller.GetComponent<NavMeshAgent>();
        agent.speed = controller.stat.MoveSpeed;

        transform = controller.transform;

        target = Manager.Game.Player.transform;

        attackSpeed = controller.stat.AttackSpeed;
    }

    public override void OnFixedUpdate()
    {
        attackCooldown += Time.fixedDeltaTime;
        if (attackCooldown > attackSpeed) { canAttack = true; attackCooldown = 0f; }
    }

    public override void OnStateEnter()
    {
        if (!DetectPlayer(10f)) isFind = false;

        attackCooldown = 0f;
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {

        if (!isFind)
        {
            Patrol();
            DetectPlayer(senseDetectRange);
        }
        else Chase();

        if (Mathf.Abs(agent.velocity.x) > 0.2f || Mathf.Abs(agent.velocity.z) > 0.2f) animator.SetBool(ENEMY_MOVE, true);
        else animator.SetBool(ENEMY_MOVE, false);
    }

    private void Patrol()
    {
        
    }

    private bool DetectPlayer(float DetectDistance)
    {
        //�ֺ� Ž��
        Collider[] cols = Physics.OverlapSphere(transform.position, DetectDistance, 1 << 10);
        foreach(Collider col in cols)
        {
            if (col.gameObject == Manager.Game.Player)
            {
                isFind = true;
                return true;
            }
        }

        // ���� ��ä�� Ž��
        Vector3 dist = target.position - transform.position;
        if(dist.magnitude <= forwardDetectRange)
        {
            float dot = Vector3.Dot(dist.normalized, transform.forward);
            float theta = Mathf.Acos(dot);
            float degree = Mathf.Rad2Deg * theta;

            if (degree <= 40f) 
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, dist.normalized, out hit, forwardDetectRange + 3);

                if(hit.collider.gameObject == target.gameObject)
                {
                    isFind = true;
                    return true;
                }
                else
                {
                    Debug.Log(hit.collider.gameObject);
                }
            }
        }

        return false;
    }

    private void Chase()
    {
        float distance = CheckDistance();
        if(distance <= attackDistance + agent.radius)
        {
            if(canAttack)
            {
                agent.SetDestination(transform.position); //���߱�

                controller.ChangeState(EnemyState.Attack);
                animator.SetTrigger(ENEMY_ATTACK);
                animator.SetBool(ENEMY_MOVE, false);
                canAttack = false;
                return;
            }
        }

        if (distance <= chaseDistance)
            agent.SetDestination(target.position);
        else
            isFind = false;
    }

    private float CheckDistance()
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target.position, path);

        //��� �Ÿ� ����� ���� ����
        Vector3[] wayPoint = new Vector3[path.corners.Length + 2];
        wayPoint[0] = transform.position;
        wayPoint[wayPoint.Length - 1] = target.position;

        float distance = 0f; //�Ÿ�
        for (int p = 0; p < path.corners.Length; p++)
        {
            wayPoint[p + 1] = path.corners[p];
            distance += Vector3.Distance(wayPoint[p], wayPoint[p + 1]);
        }

        return distance;
    }

    //enemytype�� ����� �� ����� �Լ�
    private bool CheckPlayerItemToRun()
    {
        //�ʹ� ������ ���� �ȵǴ� �̿����� ���� �߰��ؾߵɵ�
        //�÷��̾� ������ �˻� - ����: true, �´���: false

        return false;
    }
}
