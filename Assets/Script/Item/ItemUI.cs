using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }
        ToolTipManger.Instance.ShowToolTip(item.name, item.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManger.Instance.HideToolTip();


    }
}
