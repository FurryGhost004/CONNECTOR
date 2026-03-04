using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPanel : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject mainPanel;

    [Header("Tutorial Panels")]
    [SerializeField] private GameObject panel1_MovementAndSkills;
    [SerializeField] private GameObject panel2_ItemsAndShop;

    [Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button closeButton;

    [Header("Panel Indicators")]
    [SerializeField] private Image[] pageIndicators;
    [SerializeField] private Color activePageColor = Color.cyan;
    [SerializeField] private Color inactivePageColor = Color.gray;

    [Header("Panel Content - Panel 1")]
    [SerializeField] private Image wasdImage;
    [SerializeField] private Image playerRunningImage;
    [SerializeField] private TextMeshProUGUI movementText;
    [SerializeField] private Image cKeyImage;
    [SerializeField] private Image playerStealthImage;
    [SerializeField] private TextMeshProUGUI stealthText;
    [SerializeField] private Image energyBarExample;
    [SerializeField] private Image fKeyImage;
    [SerializeField] private Image interactionExampleImage;
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Panel Content - Panel 2")]
    [SerializeField] private Image keys1234Image;
    [SerializeField] private TextMeshProUGUI itemsText;
    [SerializeField] private Image shopImage;
    [SerializeField] private TextMeshProUGUI shopText;

    [Header("Animation Settings")]
    [SerializeField] private float panelTransitionSpeed = 0.3f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private int currentPanelIndex = 0;
    private GameObject[] allPanels;
    private bool isTransitioning = false;

    private void Awake()
    {
        allPanels = new GameObject[]
        {
            panel1_MovementAndSkills,
            panel2_ItemsAndShop
        };

        if (nextButton != null)
            nextButton.onClick.AddListener(NextPanel);

        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousPanel);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseTutorial);
    }

    private void OnEnable()
    {
        isTransitioning = false;
        StopAllCoroutines();

        SetupPanelTexts();
        ShowPanel(0);
    }

    private void SetupPanelTexts()
    {
        if (movementText != null)
        {
            movementText.text = "<b><color=#00EFFF>MOVEMENT</color></b>\n\n" +
                "<color=#00FFFF>W A S D</color> - Move around\n" +
                "Navigate through the city streets";
        }

        if (stealthText != null)
        {
            stealthText.text =
                "<b><color=#00EFFF>STEALTH MODE</color></b>\n\n" +
                "Press <color=#FF00FF>C</color> to activate\n" +
                "Become invisible to enemies\n" +
                "<color=#FFAA00>Energy drains over time. Using skills slows movement</color>";
        }

        if (interactionText != null)
        {
            interactionText.text =
                "<align=center><b><color=#00EFFF>INTERACT</color></b></align>\n\n" +
                "<align=left>" +
                "Press <color=#FFAA00>F</color> to:\n" +
                "• Pick up items\n" +
                "• Talk to NPCs\n" +
                "• Collect data chips\n" +
                "<color=#FFAA00>Stand close to interact</color>" +
                "</align>";
        }

        if (itemsText != null)
        {
            itemsText.text = "<align=center><b><color=#00EFFF>TOOLS & ITEMS</color></b></align>\n\n" +
                            "<align=left>" +
                            "Press <color=#00FF00>1 2 3 4</color> to use items\n" +
                            "• Slot 1: First tool\n" +
                            "• Slot 2: Second tool\n" +
                            "• Slot 3: Third tool\n" +
                            "• Slot 4: Fourth tool\n" +
                            "<color=#FFAA00>Must have item equipped first!</color>" +
                            "</align>";
        }

        if (shopText != null)
        {
            shopText.text =
                "<align=center><b><color=#00EFFF>SHOP</color></b></align>\n\n" +
                "<align=left>" +
                "Purchase tools and items here\n" +
                "• Buy essential equipment\n" +
                "• Upgrade your loadout\n" +
                "• Prepare for missions\n" +
                "<color=#FFAA00>Visit shop to gear up!</color>" +
                "</align>";
        }
    }

    public void NextPanel()
    {
        if (isTransitioning) return;

        if (currentPanelIndex < allPanels.Length - 1)
        {
            ShowPanel(currentPanelIndex + 1);
        }
    }

    public void PreviousPanel()
    {
        if (isTransitioning) return;

        if (currentPanelIndex > 0)
        {
            ShowPanel(currentPanelIndex - 1);
        }
    }

    private void ShowPanel(int panelIndex)
    {
        if (panelIndex < 0 || panelIndex >= allPanels.Length) return;

        currentPanelIndex = panelIndex;

        foreach (GameObject panel in allPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        if (allPanels[currentPanelIndex] != null)
        {
            allPanels[currentPanelIndex].SetActive(true);
            StartCoroutine(AnimatePanel(allPanels[currentPanelIndex]));
        }

        UpdateNavigationButtons();
        UpdatePageIndicators();
    }

    private System.Collections.IEnumerator AnimatePanel(GameObject panel)
    {
        isTransitioning = true;

        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = panel.AddComponent<CanvasGroup>();

        RectTransform rectTransform = panel.GetComponent<RectTransform>();

        float elapsed = 0f;
        Vector3 startScale = Vector3.one * 0.8f;
        Vector3 endScale = Vector3.one;

        canvasGroup.alpha = 0f;
        rectTransform.localScale = startScale;

        while (elapsed < panelTransitionSpeed)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / panelTransitionSpeed;
            float curved = transitionCurve.Evaluate(progress);

            canvasGroup.alpha = curved;
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, curved);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.localScale = endScale;

        isTransitioning = false;
    }

    private void UpdateNavigationButtons()
    {
        if (previousButton != null)
        {
            previousButton.interactable = (currentPanelIndex > 0);
            previousButton.gameObject.SetActive(currentPanelIndex > 0);
        }

        if (nextButton != null)
        {
            bool isLastPanel = (currentPanelIndex >= allPanels.Length - 1);
            nextButton.interactable = !isLastPanel;

            TextMeshProUGUI buttonText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isLastPanel ? "GOT IT!" : "NEXT >";
            }
        }
    }

    private void UpdatePageIndicators()
    {
        if (pageIndicators == null || pageIndicators.Length == 0) return;

        for (int i = 0; i < pageIndicators.Length; i++)
        {
            if (pageIndicators[i] != null)
            {
                pageIndicators[i].color = (i == currentPanelIndex) ? activePageColor : inactivePageColor;
            }
        }
    }

    public void CloseTutorial()
    {
        PlayerPrefs.SetInt("TutorialCompleted", 1);
        PlayerPrefs.Save();

        if (mainPanel != null)
            mainPanel.SetActive(false);
        else
            gameObject.SetActive(false);
    }

    public void ShowTutorial()
    {
        gameObject.SetActive(true);
        ShowPanel(0);
    }

    public bool HasSeenTutorial()
    {
        return PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
    }
}