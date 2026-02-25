using UnityEngine;

[CreateAssetMenu(fileName = "DeliverMessageQuest", menuName = "Quests/DeliverMessageQuest")]
public class DeliverMessageQuest: QuestData
{


    public override void ApplyObjectiveBaseOnQuestType(QuestManager questManager)
    {
        GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        if (goal != null)
        {
            GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");
            if (goals.Length>0)
            {
                if (GoalManager.Instance != null)
                {
                    GoalManager.Instance.Initialize(questManager);
                    GoalManager.Instance.SetCompletedAllRequirement(true);
                    Debug.Log($"Đã khởi tạo {goals.Length} Goal cho DeliverMessageQuest.");
                }
            }
      
        }
    }

    public override void CalculateStarRating(QuestManager questManager)
    {
        Timer timer = GameObject.FindFirstObjectByType<Timer>();

        float remainTime = timer.GetRemainTime();
        int starRating = 1;
        float timeUsed = totalTime - remainTime;
        if (remainTime > totalTime * 0.8)
        {
            starRating = 5;
        }
        else if (remainTime > totalTime * 0.6)
        {
            starRating = 4;
        }
        else if (remainTime > totalTime * 0.4)
        {
            starRating = 3;
        }
        else if (remainTime > totalTime * 0.2)
        {
            starRating = 2;
        }
        Debug.Log("Star Rating: " + starRating + ", Time Used: " + timeUsed);

        FinishQuest(questManager, starRating, timeUsed);

    }

}
