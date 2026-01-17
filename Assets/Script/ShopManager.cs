using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI Info")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemTypeText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI itemLevelText;
    public TextMeshProUGUI currentMoney;

    [Header("Shop Slots")]
    public ShopUI[] shopsSlots;
    public List<Item> itemsPool;

    private Item currentSelectedItem;
    private ShopUI currentSlot;
    //Getter
    public Item CurrentItem => currentSelectedItem;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        LoadRandomShop();
    }
    private void Update()
    {
        int money = PlayerPrefs.GetInt("Money", 0);
        currentMoney.text = money.ToString() + "$";
    }
    public void SetSelectedSlot(ShopUI slot)
    {
        currentSlot = slot;
    }
    public void ShowItemInfo(Item item)
    {
        currentSelectedItem = item;

        itemNameText.text = item.name;
        itemTypeText.text = "Type: " + item.type.ToString();
        itemDescriptionText.text = item.description.ToString();
        itemLevelText.text = "Lv: " + item.level.ToString();
        itemPriceText.text = "Price: " + item.price.ToString() + "$";
    }

    public void LoadRandomShop()
    {
        List<Item> items = new List<Item>(itemsPool);

        int count = Mathf.Min(6, items.Count);

        for (int i = 0; i < count; i++)
        {
            int random = Random.Range(0, items.Count);
            Item randomItem = items[random];
            shopsSlots[i].SetItem(randomItem);

            items.RemoveAt(random);
        }
    }

    public void Buy()
    {
        if (currentSlot == null || currentSelectedItem == null)
        {
            return;
        }

        if (currentSlot.IsSold())
        {
            return;
        }
        int money = PlayerPrefs.GetInt("Money", 0);
        if (money < currentSelectedItem.price)
        {
            Debug.Log("Not enough money!");
            return;
        }

        money -= currentSelectedItem.price;
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.Save();
        InventoryManager.Instance.AddItem(currentSelectedItem);
        currentSlot.MarkAsSold();
        ClearItemInfo();
    }

    private void ClearItemInfo()
    {
        currentSelectedItem = null;
        currentSlot = null;

        itemNameText.text = "";
        itemDescriptionText.text = "";
        itemTypeText.text = "";
        itemPriceText.text = "";
        itemLevelText.text = "";
    }
}
