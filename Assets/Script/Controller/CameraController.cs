using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    float pitch = 0f; // 수직
    float yaw = 0f; // 수평

    //현재 눈이 인스펙터에서 끌어다 쓰고 있음 수정 해야됨
    Animator animator = null;

    Transform eye = null;
    Transform spine = null;

    //아이템 픽업
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

        //플레이어가 인벤 열 때, 죽었을 때 안움직이게 조건 추가애햐됨

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
            if (hit.transform.root.tag.Equals("Item"))
            {
                //아이템은 바로 실행
                hitted.transform.root.GetComponent<Interactive>()?.InteractiveFunc();
            }
            else
            {
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
            if (hit.transform.root.tag.Equals("Item")) intText.text = $"{hit.transform.root.name} 줍기";
            else intText.text = $"{hit.transform.root.name} 열기";
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
        if (hit.transform.root.tag.Equals("Door") == false)
            Manager.Inven.nowInteractive = hitted.transform.root.GetComponent<Interactive>();
        hitted.transform.root.GetComponent<Interactive>()?.InteractiveFunc();
    }
}
