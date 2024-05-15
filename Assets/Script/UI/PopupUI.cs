using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    //�˾�ui���� ��ӹ��� ��
    public virtual void OnPopup()
    {
        Manager.UI.DisplayPopup(this);
    }
    public virtual void OnClose()
    {
        //Destroy(gameObject);
    }

    public void OnCloseEvent()
    {
        Manager.UI.ClosePopup();
    }

}
