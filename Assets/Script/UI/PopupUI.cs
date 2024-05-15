using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : MonoBehaviour
{
    //팝업ui들이 상속받을 것
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
