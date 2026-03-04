using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Scale Settings")]
    public float hoverScale = 1.1f;
    public float animationSpeed = 10f;

    [Header("Color Settings")]
    public bool changeColor = true;
    public Color normalColor = new Color(0.86f, 0.86f, 0.86f, 1f); // Trắng xám
    public Color hoverColor = new Color(0f, 0.78f, 1f, 1f); // Xanh cyan

    [Header("Icon Settings")]
    public Image iconImage; // ← THÊM MỚI: Kéo icon vào đây!

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Text buttonText;
    private TextMeshProUGUI buttonTextTMP;
    private Outline buttonOutline;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        // Tìm text component
        buttonText = GetComponentInChildren<Text>();
        buttonTextTMP = GetComponentInChildren<TextMeshProUGUI>();
        buttonOutline = GetComponent<Outline>();
    }

    void Update()
    {
        // Smooth scale animation
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale lên
        targetScale = originalScale * hoverScale;

        // Đổi màu
        if (changeColor)
        {
            if (buttonText != null)
                buttonText.color = hoverColor;

            if (buttonTextTMP != null)
                buttonTextTMP.color = hoverColor;

            if (buttonOutline != null)
                buttonOutline.effectColor = hoverColor;

            // ← THÊM MỚI: Đổi màu icon
            if (iconImage != null)
                iconImage.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Scale về
        targetScale = originalScale;

        // Màu về
        if (changeColor)
        {
            if (buttonText != null)
                buttonText.color = normalColor;

            if (buttonTextTMP != null)
                buttonTextTMP.color = normalColor;

            if (buttonOutline != null)
                buttonOutline.effectColor = normalColor;

            // ← THÊM MỚI: Icon về màu
            if (iconImage != null)
                iconImage.color = normalColor;
        }
    }
}