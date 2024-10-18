using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Item item;
    public Image itemRarityImage;
    public Image itemImage;
    public Text itemName;
    public Text itemCost;
    private ShopUI shopUI;

    public int mul = 0;
    public void ItemPriceSet()
    {
        if (item.itemType == ItemType.Equipment)
            mul = 6;
        else
            mul = 2;
    }
    public void GetShopUI(ShopUI _shopUI)
    {
        shopUI = _shopUI;
    }
    public void ItemPurchase()
    {
        // 아이템 가격보다 돈이 많은 경우
        if(Manager.Data.gold >= item.itemPrice * mul)
        {
            Item newItem = item.ItemDeepCopy();
            // 아이템 추가 시도
            if (Manager.Inven.AddItem(newItem, ItemBoxType.Inventory) != null)
            {
                // 성공시 돈을 뺌
                Manager.Data.gold -= item.itemPrice * mul;
                shopUI.audioSource.Play();
                GetComponent<Button>().interactable = false;
                // 데이터 출력
                Manager.Data.PlayerDataExport();
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
                    shopUI.audioSource.Play();
                    Destroy(gameObject);
                    // 데이터 출력
                    Manager.Data.PlayerDataExport();
                    return;
                }
            }
        }
    }
}
