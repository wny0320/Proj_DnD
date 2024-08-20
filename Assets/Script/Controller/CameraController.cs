using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    float pitch = 0f; // ����
    float yaw = 0f; // ����

    //���� ���� �ν����Ϳ��� ����� ���� ���� ���� �ؾߵ�
    Animator animator = null;

    Transform eye = null;
    Transform spine = null;

    //������ �Ⱦ�
    [SerializeField] private float reach = 1.5f;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private GameObject InteractiveUI;
    [SerializeField] private Image fImg;
    [SerializeField] private Text intText;
    [SerializeField] private Image circleProcess;
    [SerializeField] private Text processText;
    private RaycastHit hit;

    private bool isPickupActivate = false;
    private Coroutine interactingCo = null;

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        CheckInteractiveObj();
        Interactive(hit);
    }

    void LateUpdate()
    {
        SetCam();
    }

    private void SetCam()
    {
        if (Manager.Game.Player == null) return;
        else if (animator == null)
        {
            animator = Manager.Game.Player.GetComponent<Animator>();
            eye = animator.GetBoneTransform(HumanBodyBones.Head);
            spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        }

        CameraMove();
        transform.position = eye.position;
    }

    private void CameraMove()
    {
        if (!Manager.Game.isPlayerAlive) return;

        //�÷��̾ �κ� �� ��, �׾��� �� �ȿ����̰� ���� �߰������

        yaw += Input.GetAxis("Mouse X") * Manager.Input.mouseSpeed;
        pitch -= Input.GetAxis("Mouse Y") * Manager.Input.mouseSpeed;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
        eye.root.localRotation = Quaternion.Euler(0, yaw, 0);
        spine.localRotation = Quaternion.Euler(0, 0, pitch);
    }

    private void Interactive(RaycastHit hitted)
    {
        if(interactingCo != null)
        {
            intText.enabled = false;
            fImg.enabled = false;
        }

        if (isPickupActivate && Input.GetKeyDown(KeyCode.F))
        {
            if (hit.transform.tag.Equals("Item"))
            {
                //�������� �ٷ� ����
                hitted.transform.GetComponent<Interactive>()?.InteractiveFunc();
            }
            else if (hit.transform.tag.Equals("Door") || hit.transform.tag.Equals("Chest"))
            {
                //�� ���� �ڷ�ƾ
                if(interactingCo == null) interactingCo = StartCoroutine(InteractionLoading(hitted));
            }
        }
    }

    private void CheckInteractiveObj()
    {
        //Debug.DrawRay(transform.position, transform.forward * reach, Color.red);

        if (Physics.Raycast(transform.position, transform.forward, out hit, reach, layerMask))
        {
            isPickupActivate = true;
            InteractiveUI.SetActive(true);
            intText.enabled = true;
            fImg.enabled = true;
            if (hit.transform.tag.Equals("Item")) intText.text = $"{hit.transform.name} �ݱ�";
            else intText.text = $"{hit.transform.name} ����";
        }
        else
        {
            isPickupActivate = false;
            processText.enabled = false;
            circleProcess.enabled = false;
            InteractiveUI.SetActive(false);
            if (interactingCo != null)
            {
                StopCoroutine(interactingCo);
                interactingCo = null;
            }
        }
    }

    IEnumerator InteractionLoading(RaycastHit hitted)
    {
        processText.enabled = true;
        circleProcess.enabled = true;

        float t = 0f;
        circleProcess.fillAmount = 0f;
        processText.text = "0%";

        while(t <= 3f)
        {
            t += Time.deltaTime;    
            yield return null;
            processText.text = $"{Mathf.FloorToInt(t / 3 * 100)}%";
            circleProcess.fillAmount = t / 3;
        }

        processText.enabled = false;
        circleProcess.enabled = false;
        interactingCo = null;
        hitted.transform.GetComponent<Interactive>()?.InteractiveFunc();
    }
}
