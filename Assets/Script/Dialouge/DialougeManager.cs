using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialougeManager : MonoBehaviour
{
    [SerializeField] GameObject dialougePanel;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI dialougeText;
    [SerializeField] Image charaterPortrait;
    [SerializeField] GameObject continueIcon;

    public static DialougeManager Instance;
    public System.Action OnDialogueEnd = delegate { };

    private DialougeeData currentDialouge;
    private int currentLineIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;

    private void Awake()
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

    private void Update()
    {
        if (!dialougePanel.activeSelf)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }
    public void StartDialouge(DialougeeData dialougeeData, bool hasFinishedOne = false)
    {

        currentDialouge = dialougeeData;
        if (!hasFinishedOne)
        {
            currentLineIndex = 0;
        }
        else
        {
            currentLineIndex = dialougeeData.lines.Length - 1;
        }

        dialougePanel.SetActive(true);
        ShowLine();
        Time.timeScale = 0f;
    }
    public void ShowLine()
    {
        DialougeLine line = currentDialouge.lines[currentLineIndex];
        nameText.text = line.charaterName;
        charaterPortrait.sprite = line.charaterPortrait;
        dialougeText.text = "";
        continueIcon.SetActive(false);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(line.content));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        foreach (char c in text)
        {
            dialougeText.text += c;
            yield return new WaitForSecondsRealtime(0.03f);
        }
        isTyping = false;
        continueIcon.SetActive(true);
    }

    public void NextLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialougeText.text = currentDialouge.lines[currentLineIndex].content;
            isTyping = false;
            continueIcon.SetActive(true);
            return;
        }
        currentLineIndex++;
        if (currentLineIndex >= currentDialouge.lines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowLine();
        }
    }

    void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialougePanel.SetActive(false);
        Time.timeScale = 1f;

        OnDialogueEnd?.Invoke();
        OnDialogueEnd = delegate { };
    }
}
