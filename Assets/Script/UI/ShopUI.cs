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
    GameObject specialCatalog;
    [SerializeField]
    GameObject shopItemPrefab;
    [SerializeField]
    GameObject sellItemPrefab;

    List<Item> prevInvenItems;

    public AudioSource audioSource;
    private bool shopRefreshFlag = false;

    private void Start()
    {
        audioSource.clip = Global.Sound.ShopCoin;
    }
    private void Update()
    {
        ShopItemRefresh();
        ShopSellRefresh();
    }
    private void ShopSellRefresh()
    {
        if (Manager.Data.dataImportFlag == false)
            return;
        List<Item> invenItems = Manager.Inven.GetBoxItems(ItemBoxType.Inventory);
        ShopItemUI[] sellUIs = sellCatalog.GetComponentsInChildren<ShopItemUI>();
        if(prevInvenItems != null)
        {
            foreach (Item item in invenItems)
            {
                // 이전 리스트에 아이템이 포함되어 있다면 패스
                if (prevInvenItems.Contains(item))
                    continue;
                // 아이템이 없다면 새로운 버튼 생성
                else
                {
                    GameObject newSellItem = Instantiate(sellItemPrefab);
                    newSellItem.transform.SetParent(sellCatalog.transform);
                    ShopUISync(newSellItem, item, true);
                }
            }
        }
        else
        {
            foreach (Item item in invenItems)
            {
                GameObject newSellItem = Instantiate(sellItemPrefab);
                newSellItem.transform.SetParent(sellCatalog.transform);
                ShopUISync(newSellItem, item, true);
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

        // 아이템의 타입에 따른 분류
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
        // 소비 아이템 목록 갱신
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
            Item newItem = consumItemList[targetIndex].ItemDeepCopy(0f, ItemRarity.Junk);
            ShopUISync(newShopItem, newItem, false);
        }
        // prevIndex List 초기화
        // 장비 아이템 목록 갱신
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
            Item newItem = equipItemList[targetIndex].ItemDeepCopy(0f, ItemRarity.Junk);
            ShopUISync(newShopItem, newItem, false);
        }
        // prevIndex List 초기화
        // 스페셜 아이템 목록 갱신
        prevIndex = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            int targetIndex;
            int count = 0;
            while (true)
            {
                count++;
                targetIndex = Random.Range(0, equipItemList.Count);
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
            newShopItem.transform.SetParent(specialCatalog.transform);
            Item newItem = equipItemList[targetIndex].ItemDeepCopy();
            ShopUISync(newShopItem, newItem, false);
        }
        shopRefreshFlag = true;
    }
    /// <summary>
    /// 상점의 아이템 UI를 동기화시켜주는 함수
    /// </summary>
    /// <param name="_shopItemObj">생성한 ShopItemPrefab</param>
    /// <param name="_item">데이터가 들어갈 Item</param>
    /// <param name="_sellFlag">유저가 판매하는 UI라면 true</param>
    private void ShopUISync(GameObject _shopItemObj, Item _item, bool _sellFlag)
    {
        _shopItemObj.TryGetComponent<ShopItemUI>(out ShopItemUI shopItemUI);
        if (shopItemUI != null && _item != null)
        {
            shopItemUI.item = _item;
            shopItemUI.GetShopUI(this);
            shopItemUI.ItemPriceSet();
            int mul = shopItemUI.mul;
            switch (_item.itemRarity)
            {
                case ItemRarity.Junk:
                    shopItemUI.itemRarityImage.color = new Color32(255, 255, 255, 0);
                    break;
                case ItemRarity.Poor:
                    shopItemUI.itemRarityImage.color = new Color32(255, 255, 255, 60);
                    break;
                case ItemRarity.Common:
                    shopItemUI.itemRarityImage.color = new Color32(100, 255, 100, 60);
                    break;
                case ItemRarity.Rare:
                    shopItemUI.itemRarityImage.color = new Color32(90, 160, 255, 60);
                    break;
                case ItemRarity.Epic:
                    shopItemUI.itemRarityImage.color = new Color32(150, 90, 255, 60);
                    break;
                case ItemRarity.Legendary:
                    shopItemUI.itemRarityImage.color = new Color32(255, 70, 70, 60);
                    break;
            }
            shopItemUI.itemImage.sprite = Manager.Data.itemSprite[_item.itemIndex];
            shopItemUI.itemName.text = _item.itemName;
            if(_sellFlag == true)
                shopItemUI.itemCost.text = _item.itemPrice.ToString();
            else
                shopItemUI.itemCost.text = (_item.itemPrice * mul).ToString();
        }
    }
    public void ActiveConsumUI()
    {
        consumCatalog.SetActive(true);
        equipCatalog.SetActive(false);
        sellCatalog.SetActive(false);
        specialCatalog.SetActive(false);
    }

    public void ActiveEquipUI()
    {
        consumCatalog.SetActive(false);
        equipCatalog.SetActive(true);
        sellCatalog.SetActive(false);
        specialCatalog.SetActive(false);
    }

    public void ActiveSellUI()
    {
        consumCatalog.SetActive(false);
        equipCatalog.SetActive(false);
        sellCatalog.SetActive(true);
        specialCatalog.SetActive(false);
    }
    public void ActiveSpecialUI()
    {
        consumCatalog.SetActive(false);
        equipCatalog.SetActive(false);
        sellCatalog.SetActive(false);
        specialCatalog.SetActive(true);
    }
}
