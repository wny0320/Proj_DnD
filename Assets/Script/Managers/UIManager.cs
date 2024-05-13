using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Stack<PopupUI> PopUpStack = new Stack<PopupUI>();
    private int sortingOrder = 0;

    public void OnUpdate()
    {
        ClosePopup();
    }

    public void DisplayPopup(PopupUI popupUI)
    {
        PopUpStack.Push(popupUI);
        sortingOrder++;
        popupUI.GetComponent<Canvas>().sortingOrder = sortingOrder;
    }

    //esc or �ڷΰ��� ��ư ���� �� ui ó��
    public void ClosePopup()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PopUpStack.Count <= 0) return;

            PopupUI pop =  PopUpStack.Pop();
            pop.OnPopup();
            sortingOrder--;
        }
    }
}
