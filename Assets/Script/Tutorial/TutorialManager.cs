using UnityEngine;


public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] DialougeeData moveDialogue;
    [SerializeField] DialougeeData pickupDialogue;
    [SerializeField] DialougeeData throwDialogue;
    [SerializeField] DialougeeData setUpDialogue;


    [SerializeField] GameObject Movepanel;
    [SerializeField] GameObject pickupPanel;
    [SerializeField] GameObject setUPPanel;
    [SerializeField] GameObject throwPanel;
    [SerializeField] GameObject tutorialPanel;

    [SerializeField] GameObject tutorialItemPrefab1;
    [SerializeField] GameObject tutorialItemPrefab2;

    public TutorialType currentTutorial;
    public DialougeeData endTutorialDialogue;

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
    void Start()
    {
        if (TutorialSystem.IsCompleted(TutorialType.Finish))
        {
            currentTutorial = TutorialType.Finish;
            Movepanel.SetActive(false);
            return;
        }
        Debug.Log("Start Tutorial");
        if (currentTutorial == TutorialType.Move)
        {
            SpawnTutorialItem();
            currentTutorial = TutorialType.Move;
            DialougeManager.Instance.OnDialogueEnd = delegate { };
            DialougeManager.Instance.OnDialogueEnd += OnIntroEnd;
            DialougeManager.Instance.StartDialouge(moveDialogue);
        }

    }

    public void OnIntroEnd()
    {
        DialougeManager.Instance.OnDialogueEnd -= OnIntroEnd;
        Movepanel.SetActive(true);
    }
    void SpawnTutorialItem()
    {
        if (!TutorialSystem.IsCompleted(TutorialType.PickItemUP))
        {
            if (GameObject.FindWithTag("item") == null)
            {
                InventoryManager.Instance.SpawnTutorialItem(tutorialItemPrefab1, 1);
                InventoryManager.Instance.SpawnTutorialItem(tutorialItemPrefab2, 2);
            }
        }
    }
    public void OnMoveTutorialFinish()
    {
        if (currentTutorial != TutorialType.Move)
        {
            return;
        }
        Movepanel.SetActive(false);
        TutorialSystem.IsCompleted(currentTutorial);
        TutorialSystem.SetCompleted(currentTutorial);
        DialougeManager.Instance.OnDialogueEnd = delegate { };
        DialougeManager.Instance.OnDialogueEnd += StartPickupTutorial;
        DialougeManager.Instance.StartDialouge(pickupDialogue, true);
        Debug.Log("End Move Tutorial");
    }

    void StartPickupTutorial()
    {
        DialougeManager.Instance.OnDialogueEnd -= StartPickupTutorial;
        Debug.Log("Start pick up Tutorial");
        currentTutorial = TutorialType.PickItemUP;
        pickupPanel.SetActive(true);


    }

    public void OnItemPicked()
    {
        if (currentTutorial != TutorialType.PickItemUP)
        {
            return;
        }

        pickupPanel.SetActive(false);
        TutorialSystem.IsCompleted(currentTutorial);
        TutorialSystem.SetCompleted(currentTutorial);
        DialougeManager.Instance.OnDialogueEnd += StartSetUpTutorial; ;
        DialougeManager.Instance.StartDialouge(setUpDialogue);
        Debug.Log("End pick up Tutorial");
    }
    void StartSetUpTutorial()
    {
        DialougeManager.Instance.OnDialogueEnd -= StartSetUpTutorial;
        currentTutorial = TutorialType.SetUpItem;
        Debug.Log("Start set up Tutorial");
        setUPPanel.SetActive(true);


    }
    public void OnItemSet()
    {
        if (currentTutorial != TutorialType.SetUpItem)
        {
            return;
        }
        Debug.Log("End pick up Tutorial");
        setUPPanel.SetActive(false);
        TutorialSystem.IsCompleted(currentTutorial);
        TutorialSystem.SetCompleted(currentTutorial);
        DialougeManager.Instance.OnDialogueEnd += StartThrowTutorial; ;
        DialougeManager.Instance.StartDialouge(throwDialogue);
    }
    void StartThrowTutorial()
    {
        DialougeManager.Instance.OnDialogueEnd -= StartThrowTutorial;
        currentTutorial = TutorialType.ThrowItem;
        throwPanel.SetActive(true);
        Debug.Log("Start throw Tutorial");
    }

    public void OnItemThrown()
    {
        if (currentTutorial != TutorialType.ThrowItem)
        {
            return;
        }

        throwPanel.SetActive(false);
        TutorialSystem.IsCompleted(currentTutorial);
        TutorialSystem.SetCompleted(currentTutorial);
        currentTutorial = TutorialType.Finish;
        TutorialSystem.SetCompleted(currentTutorial);
        DialougeManager.Instance.StartDialouge(endTutorialDialogue);
        Debug.Log("End throw Tutorial");
        
        DialougeManager.Instance.OnDialogueEnd += ShowTutorialPanel;

    }

    void ShowTutorialPanel()
    {
        DialougeManager.Instance.OnDialogueEnd -= ShowTutorialPanel;
        currentTutorial = TutorialType.none;
        TutorialSystem.SetCompleted(currentTutorial);
        Debug.Log("current tutorial: " + currentTutorial);
        tutorialPanel.SetActive(true);

    }
}



