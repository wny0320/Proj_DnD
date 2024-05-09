using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveState : BaseState
{
    //�� ���� �� �̵�
    private bool isFind = false;
    private float followingDistance = 15f;
    private float attackDistance = 1.5f;

    private NavMeshAgent agent;
    private Transform target;
    private Transform transform;

    public EnemyMoveState(BaseController controller, Rigidbody rb = null, Animator animator = null) : base(controller, rb, animator)
    {
        agent = controller.GetComponent<NavMeshAgent>();
        transform = controller.transform;
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnStateEnter()
    {
        //Follow();
    }

    public override void OnStateExit()
    {

    }

    public override void OnStateUpdate()
    {
        if (isFind) Follow();
        else PatrolToFind();
    }

    private void PatrolToFind()
    {
        // �ٶ󺸴� ���� Ž�� ��� �� ������
    }

    private void Follow()
    {
        float distance = CheckDistance();
        if(distance <= attackDistance)
        {
            controller.ChangeState(EnemyState.Attack);
            return;
        }

        if (distance <= followingDistance)
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
}
