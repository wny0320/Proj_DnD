using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField]
    GameObject consumCatalog;
    [SerializeField]
    GameObject equipCatalog;
    [SerializeField]
    GameObject sellCatalog;
    [SerializeField]
    GameObject shopItemPrefab;
    [SerializeField]
    GameObject sellItemPrefab;

    List<Item> prevInvenItems;

    private bool shopRefreshFlag = false;
    private void Update()
    {
        ShopItemRefresh();
        ShopSellRefresh();
    }
    private void ShopSellRefresh()
    {
        if (Manager.Data.dataImportFlag == false)
            return;
        List<Item> invenItems = Manager.Inven.GetInvenItems();
        ShopItemUI[] sellUIs = sellCatalog.GetComponentsInChildren<ShopItemUI>();
        if(prevInvenItems != null)
        {
            foreach (Item item in invenItems)
            {
                // ���� ����Ʈ�� �������� ���ԵǾ� �ִٸ� �н�
                if (prevInvenItems.Contains(item))
                    continue;
                // �������� ���ٸ� ���ο� ��ư ����
                else
                {
                    GameObject newSellItem = Instantiate(sellItemPrefab);
                    newSellItem.transform.SetParent(sellCatalog.transform);
                    ShopUISync(newSellItem, item);
                }
            }
        }
        else
        {
            foreach (Item item in invenItems)
            {
                GameObject newSellItem = Instantiate(sellItemPrefab);
                newSellItem.transform.SetParent(sellCatalog.transform);
                ShopUISync(newSellItem, item);
            }
        }
        foreach(ShopItemUI sellItemUI in sellUIs)
        {
            if(invenItems.Contains(sellItemUI.item) == false)
            {
                Destroy(sellItemUI.gameObject);
            }
        }
        prevInvenItems = invenItems;
    }

    private void ShopItemRefresh()
    {
        if (shopRefreshFlag == true)
            return;
        if (Manager.Data.dataImportFlag == false)
            return;
        Dictionary<string, Item> itemdata = Manager.Data.itemData;

        List<Item> consumItemList = new List<Item>();
        List<Item> equipItemList = new List<Item>();
        List<int> prevIndex = new List<int>();

        // �������� Ÿ�Կ� ���� �з�
        foreach(var item in itemdata.Values)
        {
            if(item.itemType == ItemType.Consumable)
            {
                consumItemList.Add(item);
            }
            if(item.itemType == ItemType.Equipment)
            {
                equipItemList.Add(item);
            }
        }
        // �Һ� ������ ��� ����
        for(int i = 0; i < 2; i++)
        {
            int targetIndex;
            int count = 0;
            while (true)
            {
                count++;
                targetIndex = Random.Range(0, consumItemList.Count);
                if (prevIndex.Contains(targetIndex))
                {
                    if (count > 10)
                        break;
                    continue;
                }
                else
                {
                    prevIndex.Add(targetIndex);
                    break;
                }
            }
            GameObject newShopItem = Instantiate(shopItemPrefab);
            newShopItem.transform.SetParent(consumCatalog.transform);
            ShopUISync(newShopItem, consumItemList[targetIndex]);
        }
        // prevIndex List �ʱ�ȭ
        // ��� ������ ��� ����
        prevIndex = new List<int>();
        for(int i = 0; i < 4; i++)
        {
            int targetIndex;
            int count = 0;
            while (true)
            {
                count++;
                targetIndex = Random.Range(0, equipItemList.Count);
                if (prevIndex.Contains(targetIndex))
                {
                    if(count > 10)
                        break;
                    continue;
                }
                else
                {
                    prevIndex.Add(targetIndex);
                    break;
                }
            }
            GameObject newShopItem = Instantiate(shopItemPrefab);
            newShopItem.transform.SetParent(equipCatalog.transform);
            ShopUISync(newShopItem, equipItemList[targetIndex]);
        }
        shopRefreshFlag = true;
    }
    /// <summary>
    /// ������ ������ UI�� ����ȭ�����ִ� �Լ�
    /// </summary>
    /// <param name="_shopItemObj">������ ShopItemPrefab</param>
    /// <param name="_item">�����Ͱ� �� Item</param>
    private void ShopUISync(GameObject _shopItemObj, Item _item)
    {
        _shopItemObj.TryGetComponent<ShopItemUI>(out ShopItemUI shopItemUI);
        if (shopItemUI != null)
        {
            shopItemUI.item = _item;
            shopItemUI.itemImage.sprite = Manager.Data.itemSprite[_item.itemIndex];
            shopItemUI.itemName.text = _item.itemName;
            shopItemUI.itemCost.text = _item.itemPrice.ToString();
        }
    }
    public void ShopItemSell()
    {

    }
    public void ActiveConsumUI()
    {
        consumCatalog.SetActive(true);
        equipCatalog.SetActive(false);
        sellCatalog.SetActive(false);
    }

    public void ActiveEquipUI()
    {
        consumCatalog.SetActive(false);
        equipCatalog.SetActive(true);
        sellCatalog.SetActive(false);
    }

    public void ActiveSellUI()
    {
        consumCatalog.SetActive(false);
        equipCatalog.SetActive(false);
        sellCatalog.SetActive(true);
    }
}
