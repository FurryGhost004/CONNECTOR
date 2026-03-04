using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance;
    private QuestManager manager;
    private bool isReached = false;
    private bool isCompletedAllRequirement = false;

    public void Initialize(QuestManager qm)
    {
        manager = qm;
        isReached = false;
        isCompletedAllRequirement = false;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCompletedAllRequirement(bool status)
    {
        isCompletedAllRequirement = status;
        if (isReached && isCompletedAllRequirement)
        {
            CheckCompletion();
        }

    }
    public void OnGoalReached(GameObject goalObject)
    {
        if (manager == null) manager = QuestManager.Instance;
        QuestData currentQuest = manager.currentQuestData;
        isReached = true;
        if (currentQuest.questType == QuestType.DeliverItem)
        {
            if (InventoryManager.Instance.HasQuestItem())
            {
                InventoryManager.Instance.UseQuestItem();
                Debug.Log("Delivered the item to the goal.");
                goalObject.SetActive(false);
                CheckAllGoalsDelivered();
            }
        }
        else if (currentQuest.questType == QuestType.DeliverMessage)
        {
            // For simplicity, we assume the message is always delivered successfully.
            Debug.Log("Delivered the message to the goal.");
            isReached = true;
            SetCompletedAllRequirement(true);
            Debug.Log("All goals have been interacted!");
            CheckCompletion();
        }
        else if (currentQuest.questType == QuestType.InteractWithObject)
        {
            // For simplicity, we assume the interaction is always successful.
            Debug.Log("Interacted with the goal object.");
            CheckAllObjectsInteracted();
        }
        else
        {
            Debug.LogWarning("Unknown quest type. No specific action taken on goal reach.");
        }


    }

    void CheckCompletion()
    {

        if (isCompletedAllRequirement && isReached)
        {
            Debug.Log("All Requirements Completed. Quest Goal Achieved!");
            QuestManager.Instance.ReachGoal();
        }
    }

    void CheckAllGoalsDelivered()
    {
        GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");

        if (goals.Length < 1)
        {
            isReached = true;
            SetCompletedAllRequirement(true);

            Debug.Log("All goals have been delivered!");

        }
        else
        {
            isReached = false;
            Debug.Log($"There are still {goals.Length} goals remaining.");
        }

    }

    void CheckAllObjectsInteracted()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("QuestObject");
        int count = 0;
        foreach (var obj in objects)
        {

            if (obj.activeInHierarchy)
            {
                count++;
            }
        }
        if (count < 1)
        {
            isReached = true;
            SetCompletedAllRequirement(true);
            Debug.Log("All goals have been interacted!");
            CheckCompletion();
        }
        else
        {
            isReached = false;
            Debug.Log($"There are still {objects.Length} goals remaining.");
        }
    }
}
