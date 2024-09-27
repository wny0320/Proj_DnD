using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Item item;
    public Image itemImage;
    public Text itemName;
    public Text itemCost;

    public void ItemPurchase()
    {
        // ������ ���ݺ��� ���� ���� ���
        if(Manager.Data.gold >= item.itemPrice)
        {
            // ������ �߰� �õ�
            if(Manager.Inven.AddItem(item.ItemDeepCopy()) == true)
            {
                // ������ ���� ��
                Manager.Data.gold -= item.itemPrice;
                GetComponent<Button>().interactable = false;
            }
            else
            {
                // ���н� �׳� return
                return;
            }
        }
    }
    public void ItemSell()
    {
        List<SlotLine> invenSlotLines = Manager.Inven.invenSlotLines;
        for (int y = 0; y < invenSlotLines.Count; y++)
        {
            for(int x = 0; x < invenSlotLines[y].mySlots.Count; x++)
            {
                if(invenSlotLines[y].mySlots[x].slotItem == item)
                {
                    Manager.Inven.DeleteInvenItem(invenSlotLines[y].mySlots[x], Manager.Inven.invenSlotLines);
                    Manager.Data.gold += item.itemPrice;
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}