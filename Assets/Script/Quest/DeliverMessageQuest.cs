using UnityEngine;

[CreateAssetMenu(fileName = "DeliverMessageQuest", menuName = "Quests/DeliverMessageQuest")]
public class DeliverMessageQuest: QuestData
{
    bool isGoalReached = false;

    public override void ApplyObjectiveBaseOnQuestType(QuestManager questManager)
    {
        GameObject goal = GameObject.FindGameObjectWithTag("Goal");
        if (goal != null)
        {
            GoalManager goalComponent = goal.GetComponent<GoalManager>();
            if (goalComponent == null)
            {
                goalComponent = goal.AddComponent<GoalManager>();
            }
                goalComponent.Initialize(questManager);
                goalComponent.SetCompletedAllRequirement(true);
                Debug.Log("GoalManager component added to Goal object for DeliverMessageQuest.");
            
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
