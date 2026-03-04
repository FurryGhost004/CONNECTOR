using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractWithObject", menuName = "Quests/InteractWithObjectQuest")]
public class InteractWithObject : QuestData
{
    public List<GameObject> objectsToInteractPrefab;
    public List<Vector3> objectsToInteractSpawnPoints;
    int interactFailCount = 0;
    
    

    public override void ApplyObjectiveBaseOnQuestType(QuestManager questManager)
    {
        if (objectsToInteractPrefab != null && objectsToInteractSpawnPoints != null)
        {
            for (int i = 0; i < objectsToInteractSpawnPoints.Count; i++)
            {
                GameObject prefab = objectsToInteractPrefab[i % objectsToInteractPrefab.Count];
                Vector3 spawnPos = objectsToInteractSpawnPoints[i];

                GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
                obj.tag = "QuestObject";
            if (obj.GetComponent<QuestObject>() == null)
            {
                obj.AddComponent<QuestObject>();
            }
        }
        
        // Khởi tạo GoalManager
        GoalManager.Instance.Initialize(questManager);
        GoalManager.Instance.SetCompletedAllRequirement(false);
        }
    }
    
    public override void CalculateStarRating(QuestManager questManager)
    {
        Timer timer = GameObject.FindFirstObjectByType<Timer>();

        float remainTime = timer.GetRemainTime();
        int starRating = 1;
        float timeUsed = totalTime - remainTime;
        if (remainTime > totalTime * 0.8 )
        {
            starRating = 5;
        }
        else if (remainTime > totalTime * 0.6 )
        {
            starRating = 4;
        }
        else if (remainTime > totalTime * 0.4 )
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
