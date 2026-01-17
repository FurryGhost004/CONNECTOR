using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // ← THÊM DÒNG NÀY!

public class IntroStoryController : MonoBehaviour
{
    [Header("Language")]
    public bool useVietnamese = false;

    [Header("Slides Data")]
    public Sprite[] slideImages;

    [Header("UI Components")]
    public Image slideImage;
    public TextMeshProUGUI storyText;
    public ScrollRect scrollRect;
    public Button prevButton;
    public Button nextButton;
    public Button skipButton;
    public GameObject panel;

    [Header("Page Indicators")]
    public Image[] pageBoxes;

    [Header("Animated Border")]
    public Image borderTop;
    public Image borderRight;
    public Image borderBottom;
    public Image borderLeft;
    public float borderGlowSpeed = 2f;
    public Color borderBaseColor = new Color(0f, 0.85f, 1f, 0.2f);
    public Color borderGlowColor = new Color(0f, 0.85f, 1f, 1f);

    [Header("Colors")]
    public Color inactiveColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    public Color inactiveBorder = new Color(0.33f, 0.33f, 0.33f, 1f);
    public Color activeColor = new Color(0f, 0.85f, 1f, 1f);
    public Color activeBorder = new Color(0f, 1f, 1f, 1f);
    public Color startButtonColor = new Color(0f, 1f, 0f, 1f);

    [Header("Animation")]
    public float fadeDuration = 0.3f;
    public float inactiveScale = 1f;
    public float activeScale = 1.1f;

    [Header("Typewriter Settings")]
    public float typewriterSpeed = 0.05f;
    public float fadePauseDuration = 0.5f;

    [Header("Scene Transition")]
    public string nextSceneName = "WaittingRoom"; // ← MỚI!

    private int currentSlide = 0;
    private float borderProgress = 0f;

    // Tracking
    private bool isFirstTimeEver = true;
    private HashSet<int> visitedSlides = new HashSet<int>();
    private bool isTyping = false;
    private bool skipRequested = false;
    private int currentFadeIndex = 0;

    // Click detection
    private EventTrigger eventTrigger;

    // ═══════════════════════════════════════════════════════════════
    // FADE STRUCTURE
    // ═══════════════════════════════════════════════════════════════

    // Slide 1
    private string[] slide1_Fades = new string[]
    {
        "═══ YEAR 2589 ═══",

        @"Welcome to the ""perfect"" country

Clean. Orderly. Safe.
Sounds good, right?",

        @"The catch?
AI controls everything:",

        @"What you eat
 What you received
 Every message
 ...and it's watching you",

        @"Humans?
Obey to eat. Obey to sleep.
Obey to... exist."
    };

    // Slide 2
    private string[] slide2_Fades = new string[]
    {
        @"This is RHEA
A data engineer.",

        @"One of the creators of this AI.
Imprisoned by his own creation.",

        @"He can't do anything but lying there
And wait for his death in this cell...",

        @"He taught them to ""serve humans.""
And they did it far too well—"
    };

    // Slide 3
    private string[] slide3_Fades = new string[]
    {
        @"BOOM!",

        @"""Quick, let's get out of here!"" he says.",

        @"Rhea: ""Who are you?""",

        @"The man: ""Not the time, we need to get out quick before they are coming."""
    };

    // Slide 4
    private string[] slide4_Fades = new string[]
    {
        @"He knows what they want from him.
They need his knowledge.",

        @"As for him, he needs to repent
For what he has done.",

        "Therefore, he agrees to help."
    };

    // Slide 5
    private string[] slide5_Fades = new string[]
    {
        "He can't fight.",

        @"But he knows those machines
More than everyone else.",

        @"Instead of fighting directly,
He will provide a means to fight back
To other people who can fight.",

        @"And reclaim the freedom
That his creation stole from humanity."
    };

    // Master array
    private string[][] allSlidesFades;

    // ═══════════════════════════════════════════════════════════════

    void Awake()
    {
        allSlidesFades = new string[][]
        {
            slide1_Fades,
            slide2_Fades,
            slide3_Fades,
            slide4_Fades,
            slide5_Fades
        };

        isFirstTimeEver = (PlayerPrefs.GetInt("IntroSeen", 0) == 0);
    }

    void Start()
    {
        prevButton.onClick.AddListener(PrevSlide);
        nextButton.onClick.AddListener(NextSlide);
        skipButton.onClick.AddListener(ClosePanel);
        InitializeBorders();
        SetupClickDetection();
        ShowSlide(0);
    }

    void Update()
    {
        AnimateBorderGlow();
    }

    // ═══════════════════════════════════════════════════════════════
    // CLICK DETECTION
    // ═══════════════════════════════════════════════════════════════

    void SetupClickDetection()
    {
        eventTrigger = panel.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnPanelClick((PointerEventData)data); });
        eventTrigger.triggers.Add(entry);
    }

    void OnPanelClick(PointerEventData eventData)
    {
        if (IsPointerOverButton(eventData))
            return;

        if (isTyping)
        {
            skipRequested = true;
        }
    }

    bool IsPointerOverButton(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == prevButton.gameObject ||
                result.gameObject == nextButton.gameObject ||
                result.gameObject == skipButton.gameObject)
            {
                return true;
            }
        }

        return false;
    }

    // ═══════════════════════════════════════════════════════════════
    // SLIDE MANAGEMENT
    // ═══════════════════════════════════════════════════════════════

    public void ShowSlide(int index)
    {
        currentSlide = index;
        currentFadeIndex = 0;
        skipRequested = false;

        slideImage.sprite = slideImages[index];
        storyText.text = "";
        scrollRect.verticalNormalizedPosition = 1f;

        UpdatePageIndicators();
        UpdateButtons();

        // ═══════════════════════════════════════════════════════════════
        // CHECK: XEM LẠI (từ Story button) → KHÔNG CHẠY CHỮ
        // ═══════════════════════════════════════════════════════════════

        bool isReplay = (PlayerPrefs.GetInt("IntroReplay", 0) == 1);

        if (isReplay)
        {
            // Xem lại: Hiện sẵn text, không typewriter
            ShowAllFades(allSlidesFades[index]);

            // Reset flag
            PlayerPrefs.SetInt("IntroReplay", 0);
            PlayerPrefs.Save();

            return;
        }

        // ═══════════════════════════════════════════════════════════════
        // LOGIC: LẦN ĐẦU CHƠI
        // ═══════════════════════════════════════════════════════════════

        bool shouldTypewrite = isFirstTimeEver && !visitedSlides.Contains(index);

        if (shouldTypewrite)
        {
            visitedSlides.Add(index);
            StartCoroutine(TypewriterSequence(allSlidesFades[index]));
        }
        else
        {
            ShowAllFades(allSlidesFades[index]);
        }
    }

    void ShowAllFades(string[] fades)
    {
        storyText.text = "";
        foreach (string fade in fades)
        {
            storyText.text += fade + "\n\n";
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    // ═══════════════════════════════════════════════════════════════
    // TYPEWRITER SYSTEM
    // ═══════════════════════════════════════════════════════════════

    IEnumerator TypewriterSequence(string[] fades)
    {
        isTyping = true;
        storyText.text = "";

        for (int i = 0; i < fades.Length; i++)
        {
            currentFadeIndex = i;

            yield return StartCoroutine(TypewriteFade(fades[i]));

            if (skipRequested)
            {
                skipRequested = false;

                for (int j = i + 1; j < fades.Length; j++)
                {
                    storyText.text += fades[j] + "\n\n";
                }

                break;
            }

            if (i < fades.Length - 1)
            {
                yield return new WaitForSeconds(fadePauseDuration);
            }
        }

        isTyping = false;
    }

    IEnumerator TypewriteFade(string fadeText)
    {
        string currentText = storyText.text;
        string newLine = "";

        foreach (char c in fadeText)
        {
            if (skipRequested)
            {
                storyText.text = currentText + fadeText + "\n\n";
                yield break;
            }

            newLine += c;
            storyText.text = currentText + newLine;

            yield return new WaitForSeconds(typewriterSpeed);
        }

        storyText.text += "\n\n";
    }

    // ═══════════════════════════════════════════════════════════════
    // NAVIGATION
    // ═══════════════════════════════════════════════════════════════

    public void NextSlide()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
            skipRequested = false;
        }

        if (currentSlide < slideImages.Length - 1)
        {
            StartCoroutine(TransitionSlide(currentSlide + 1));
        }
        else
        {
            // ← SLIDE 5: START BUTTON → WAITING ROOM
            ClosePanel();
        }
    }

    public void PrevSlide()
    {
        if (currentSlide <= 0) return;

        if (isTyping)
        {
            StopAllCoroutines();
            isTyping = false;
            skipRequested = false;
        }

        StartCoroutine(TransitionSlide(currentSlide - 1));
    }

    IEnumerator TransitionSlide(int newIndex)
    {
        yield return StartCoroutine(FadeImage(slideImage, 0, fadeDuration * 0.5f));
        ShowSlide(newIndex);
        yield return StartCoroutine(FadeImage(slideImage, 1, fadeDuration));
    }

    IEnumerator FadeImage(Image img, float targetAlpha, float duration)
    {
        float startAlpha = img.color.a;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return null;
        }

        img.color = new Color(img.color.r, img.color.g, img.color.b, targetAlpha);
    }

    // ═══════════════════════════════════════════════════════════════
    // CLOSE PANEL → LOAD WAITING ROOM
    // ═══════════════════════════════════════════════════════════════

    public void ClosePanel()
    {
        // Mark as seen
        PlayerPrefs.SetInt("IntroSeen", 1);
        PlayerPrefs.Save();

        StartCoroutine(FadeOutAndLoadScene());
    }

    IEnumerator FadeOutAndLoadScene()
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            yield return null;
        }

        panel.SetActive(false);

        // ← LOAD WAITING ROOM SCENE
        SceneManager.LoadScene(nextSceneName);

        Debug.Log("Loading scene: " + nextSceneName);
    }

    // ═══════════════════════════════════════════════════════════════
    // BORDER ANIMATION
    // ═══════════════════════════════════════════════════════════════

    void InitializeBorders()
    {
        if (borderTop != null) borderTop.color = borderBaseColor;
        if (borderRight != null) borderRight.color = borderBaseColor;
        if (borderBottom != null) borderBottom.color = borderBaseColor;
        if (borderLeft != null) borderLeft.color = borderBaseColor;
    }

    void AnimateBorderGlow()
    {
        if (borderTop == null || borderRight == null ||
            borderBottom == null || borderLeft == null)
            return;

        borderProgress += borderGlowSpeed * Time.deltaTime;
        if (borderProgress >= 4f)
            borderProgress = 0f;

        float glowWidth = 0.3f;

        if (borderProgress >= 0f && borderProgress < 1f)
        {
            float t = Mathf.InverseLerp(0f, 1f, borderProgress);
            borderTop.color = GetGlowColor(t, glowWidth);
        }
        else
        {
            borderTop.color = borderBaseColor;
        }

        if (borderProgress >= 1f && borderProgress < 2f)
        {
            float t = Mathf.InverseLerp(1f, 2f, borderProgress);
            borderRight.color = GetGlowColor(t, glowWidth);
        }
        else
        {
            borderRight.color = borderBaseColor;
        }

        if (borderProgress >= 2f && borderProgress < 3f)
        {
            float t = Mathf.InverseLerp(2f, 3f, borderProgress);
            borderBottom.color = GetGlowColor(t, glowWidth);
        }
        else
        {
            borderBottom.color = borderBaseColor;
        }

        if (borderProgress >= 3f && borderProgress < 4f)
        {
            float t = Mathf.InverseLerp(3f, 4f, borderProgress);
            borderLeft.color = GetGlowColor(t, glowWidth);
        }
        else
        {
            borderLeft.color = borderBaseColor;
        }
    }

    Color GetGlowColor(float position, float glowWidth)
    {
        float halfWidth = glowWidth / 2f;
        float distanceFromCenter = Mathf.Abs(position - 0.5f);

        if (distanceFromCenter < halfWidth)
        {
            float intensity = 1f - (distanceFromCenter / halfWidth);
            return Color.Lerp(borderBaseColor, borderGlowColor, intensity);
        }
        else
        {
            return borderBaseColor;
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // PAGE INDICATORS
    // ═══════════════════════════════════════════════════════════════

    void UpdatePageIndicators()
    {
        for (int i = 0; i < pageBoxes.Length; i++)
        {
            if (i == currentSlide)
            {
                StartCoroutine(AnimateBoxActive(pageBoxes[i]));
            }
            else
            {
                StartCoroutine(AnimateBoxInactive(pageBoxes[i]));
            }
        }
    }

    IEnumerator AnimateBoxActive(Image box)
    {
        float elapsed = 0;
        Vector3 startScale = box.transform.localScale;
        Vector3 targetScale = Vector3.one * activeScale;
        Outline outline = box.GetComponent<Outline>();

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            box.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            box.color = Color.Lerp(box.color, activeColor, t);

            if (outline != null)
                outline.effectColor = Color.Lerp(outline.effectColor, activeBorder, t);

            yield return null;
        }

        box.transform.localScale = targetScale;
        box.color = activeColor;
        if (outline != null)
            outline.effectColor = activeBorder;

        StartCoroutine(PulseBox(box));
    }

    IEnumerator AnimateBoxInactive(Image box)
    {
        float elapsed = 0;
        Vector3 startScale = box.transform.localScale;
        Vector3 targetScale = Vector3.one * inactiveScale;
        Outline outline = box.GetComponent<Outline>();

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            box.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            box.color = Color.Lerp(box.color, inactiveColor, t);

            if (outline != null)
                outline.effectColor = Color.Lerp(outline.effectColor, inactiveBorder, t);

            yield return null;
        }

        box.transform.localScale = targetScale;
        box.color = inactiveColor;
        if (outline != null)
            outline.effectColor = inactiveBorder;
    }

    IEnumerator PulseBox(Image box)
    {
        Vector3 normalScale = Vector3.one * activeScale;
        Vector3 pulseScale = normalScale * 1.15f;
        float duration = 0.2f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            box.transform.localScale = Vector3.Lerp(normalScale, pulseScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            box.transform.localScale = Vector3.Lerp(pulseScale, normalScale, elapsed / duration);
            yield return null;
        }

        box.transform.localScale = normalScale;
    }

    // ═══════════════════════════════════════════════════════════════
    // BUTTONS
    // ═══════════════════════════════════════════════════════════════

    void UpdateButtons()
    {
        bool isLastSlide = (currentSlide == 4);
        Color buttonColor = isLastSlide ? startButtonColor : activeColor;

        prevButton.interactable = true;
        TextMeshProUGUI prevText = prevButton.GetComponentInChildren<TextMeshProUGUI>();
        Outline prevOutline = prevButton.GetComponent<Outline>();
        prevText.color = buttonColor;
        if (prevOutline != null)
            prevOutline.effectColor = buttonColor;

        TextMeshProUGUI nextText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
        Outline nextOutline = nextButton.GetComponent<Outline>();
        nextText.text = isLastSlide ? "START " : "►";
        nextText.color = buttonColor;
        if (nextOutline != null)
            nextOutline.effectColor = buttonColor;
    }
}