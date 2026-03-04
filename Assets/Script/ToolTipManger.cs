using UnityEngine;

public class ToolTipManger : MonoBehaviour
{
    public Canvas parentCanvas;
    public RectTransform TooltipTranform;
    public Vector2 offset = new Vector2(0, 200f);
    public static ToolTipManger Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        TooltipTranform.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!TooltipTranform.gameObject.activeSelf) return;
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            parentCanvas.worldCamera,
            out movePos
        );

        TooltipTranform.anchoredPosition = movePos + offset;
    }

    public void ShowToolTip(string title, string description)
    {
        TooltipTranform.gameObject.SetActive(true);

        TooltipTranform.Find("BackGround/Title")
            .GetComponent<TMPro.TextMeshProUGUI>().text = title;

        TooltipTranform.Find("BackGround/Description")
            .GetComponent<TMPro.TextMeshProUGUI>().text = description;
    }

    public void HideToolTip()
    {
        TooltipTranform.gameObject.SetActive(false);
    }
}
