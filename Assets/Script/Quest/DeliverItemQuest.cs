using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeliverItemQuest", menuName = "Quests/DeliverItemQuest")]
public class DeliverItemQuest : QuestData
{
    
    public int keyItemAmount;
    public List<Item> itemsToDeliver;
    public override void ApplyObjectiveBaseOnQuestType(QuestManager questManager)
    {
       foreach (var item in itemsToDeliver)
        {
            InventoryManager.Instance.AddItem(item);
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
