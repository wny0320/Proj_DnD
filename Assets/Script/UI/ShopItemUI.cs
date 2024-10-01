using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Item item;
    public Image itemRarityImage;
    public Image itemImage;
    public Text itemName;
    public Text itemCost;

    public void ItemPurchase()
    {
        // 아이템 가격보다 돈이 많은 경우
        if(Manager.Data.gold >= item.itemPrice * 2)
        {
            Item newItem = item.ItemDeepCopy();
            // 아이템 추가 시도
            if (Manager.Inven.AddItem(newItem, ItemBoxType.Inventory) != null)
            {
                // 성공시 돈을 뺌
                Manager.Data.gold -= item.itemPrice * 2;
                GetComponent<Button>().interactable = false;
            }
            else
            {
                // 실패시 그냥 return
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
                    Manager.Inven.DeleteBoxItem(invenSlotLines[y].mySlots[x], ItemBoxType.Inventory);
                    Manager.Data.gold += item.itemPrice;
                    Destroy(gameObject);
                    return;
                }
            }
        }
    }
}
