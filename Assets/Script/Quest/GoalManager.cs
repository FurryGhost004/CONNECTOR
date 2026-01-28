using UnityEngine;

public class GoalManager : MonoBehaviour
{
    private QuestManager manager;
    private bool isReached = false;
    private bool isCompletedAllRequirement = false;

    public void Initialize(QuestManager qm)
    {
        manager = qm;
    }
    public void SetCompletedAllRequirement(bool status)
    {
        isCompletedAllRequirement = status;
        if (isReached && isCompletedAllRequirement)
        {
            CheckCompletion();
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isReached)
        {
            isReached = true;
            CheckCompletion();
        }
    }
    void CheckCompletion()
    {

        if (isCompletedAllRequirement && isReached)
        {
            QuestManager.Instance.ReachGoal();
        }
    }
}
