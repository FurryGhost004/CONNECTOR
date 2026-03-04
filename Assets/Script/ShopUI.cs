using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public Item item;
    public Image icon;
    public Button button;
    public GameObject sold;
    private bool isSold = false;
    void Start()
    {
        button.onClick.AddListener(OnClickSlot);
        sold.SetActive(false);
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        sold.SetActive(false);
        isSold = false;
    }
    void OnClickSlot()
    {
        if (item == null || isSold)
        {
            return;
        }

        ShopManager.Instance.ShowItemInfo(item);
        ShopManager.Instance.SetSelectedSlot(this);
    }

    public void MarkAsSold()
    {
        isSold = true;
        sold.SetActive(true);

        icon.color = new Color(1, 1, 1, 0.3f);
    }

    public bool IsSold()
    {
        return isSold;
    }
}
