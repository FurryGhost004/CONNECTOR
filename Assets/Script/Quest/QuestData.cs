using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public enum QuestType
{
    DeliverItem,
    DeliverMessage,
    InteractWithObject,

}

public abstract class QuestData : ScriptableObject
{


    [Header("Level Settings")]
    public QuestType questType;
    public int difficultyLevel;
    public int numberOfKeys;
    public float totalTime;
    public int rewardPerStar;
    public bool hasDarkness;
    public List<Item> itemsInChest;

    [Header("Spawn Settings")]
    public int enemyCount;
    public List<GameObject> enemyPrefabs;
    public GameObject goalPrefab;
    public GameObject keyPrefab;
    public Vector3 playerSpawnPoint;
    public Vector3 goalSpawnPoint;
    public List<Vector3> keySpawnPoints;

    [Header("Quest Description")]
    public string questName;
    public string difficultyDescription;

    [TextArea]
    public string questDescription;

    public abstract void ApplyObjectiveBaseOnQuestType(QuestManager questManager);
    public abstract void CalculateStarRating(QuestManager questManager);
    protected void FinishQuest(QuestManager questManager, int starRating, float timeUsed)
    {
        int totalReward = starRating * rewardPerStar;
        int currentMoney = PlayerPrefs.GetInt("Money", 0);
        PlayerPrefs.SetInt("Money", currentMoney + totalReward);
        PlayerPrefs.Save();

        string key = questName + "_Star";
        int bestStar = PlayerPrefs.GetInt(key, 0);
        if (starRating > bestStar)
        {
            PlayerPrefs.SetInt(key, starRating);
            PlayerPrefs.Save();
        }

        string timeKey = questName + "_BestTime";
        float previousBestTime = PlayerPrefs.GetFloat(timeKey, float.MaxValue);
        if (timeUsed < previousBestTime)
        {
            PlayerPrefs.SetFloat(timeKey, timeUsed);
            PlayerPrefs.Save();
        }

        if (VictoryUI.Instance != null)
        {
            VictoryUI.Instance.ShowVictoryPanel(starRating, totalReward);
        }
    }

}
