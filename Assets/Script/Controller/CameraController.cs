using DG.Tweening;
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
        if (!Manager.Game.isPlayerAlive || Manager.Inven.canvasVisualFlag) return;

        //�÷��̾ �κ� �� ��, �׾��� �� �ȿ����̰� ���� �߰������

        yaw += Input.GetAxis("Mouse X") * Manager.Input.mouseSpeed;
        pitch -= Input.GetAxis("Mouse Y") * Manager.Input.mouseSpeed;

        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
        eye.root.localRotation = Quaternion.Euler(0, yaw, 0);
        spine.localRotation = Quaternion.Euler(pitch, 0, 0);
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
            Interactive inter = CheckInteractive(hitted.transform);
            if (inter == null) return;

            if (inter.tag.Equals("Item"))
            {
                //�������� �ٷ� ����
                inter.InteractiveFunc();
            }
            else
            {
                if(interactingCo == null)
                    interactingCo = StartCoroutine(InteractionLoading(inter));
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
            if (CheckInteractive(hit.transform).tag.Equals("Item")) intText.text = $"{hit.transform.root.GetComponent<Item3D>().myItem.itemName} �ݱ�";
            else if (CheckInteractive(hit.transform).tag.Equals("Monster")) intText.text = "��ü ��Ż";
            else if (CheckInteractive(hit.transform).tag.Equals("Door")) intText.text = "�� ����";
            else if (CheckInteractive(hit.transform).tag.Equals("Chest")) intText.text = "���� ����";
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

    IEnumerator InteractionLoading(Interactive inter)
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
        if (inter.tag.Equals("Door") == false || inter.tag.Equals("Torch"))
            Manager.Inven.nowInteractive = inter;
        inter.InteractiveFunc();
    }

    private Interactive CheckInteractive(Transform trans)
    {
        if (trans.GetComponent<Interactive>() == null)
        {
            if (trans.parent == null) return null;
            return CheckInteractive(trans.parent);
        }
        else
            return trans.GetComponent<Interactive>();
    }
}
