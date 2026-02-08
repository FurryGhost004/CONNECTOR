using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest")]
    public QuestData currentQuestData;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "WaittingRoom")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(-4.8f, -2.6f, 0);
            }
            return;
        }
        if (currentQuestData == null)
        {
            return;
        }
        StopAllCoroutines();
        StartCoroutine(DeferredApplyQuestData());

    }
    IEnumerator DeferredApplyQuestData()
    {       
        yield return new WaitForEndOfFrame();

        ApplyQuestData();
    }
    void ApplyQuestData()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = currentQuestData.playerSpawnPoint;

        }

        EnemySpawner enemySpawner = FindFirstObjectByType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.Initialize(currentQuestData);
        }

        Timer timer = FindFirstObjectByType<Timer>();
        if (timer != null)
        {
            timer.Initialize(currentQuestData.totalTime);
        }
        if (currentQuestData.goalPrefab != null)
        {
            Instantiate(currentQuestData.goalPrefab, currentQuestData.goalSpawnPoint, Quaternion.identity);
        }
        Debug.Log(currentQuestData.questName + " Data Applied");

        GameObject darkness = GameObject.FindGameObjectWithTag("Darkness");
        if (!currentQuestData.hasDarkness)
        {
            darkness.SetActive(false);

        }

        Chest[] allChests = FindObjectsByType<Chest>(FindObjectsSortMode.None);
        int lockedChestsCount = 0;

        foreach (Chest chest in allChests)
        {
            int randomIndex = Random.Range(0, currentQuestData.itemsInChest.Count);
            Item randomItem = currentQuestData.itemsInChest[randomIndex];
            if (randomItem == null) continue;
            bool needsKey = Random.Range(0, 2) == 1;
            if (needsKey)
            {
                lockedChestsCount++;
            }
            chest.Initialize(randomItem, needsKey);
        }
        SpawnKeys(lockedChestsCount);
        if (currentQuestData.goalPrefab != null)
        {
            GameObject spawnedGoal = Instantiate(currentQuestData.goalPrefab, currentQuestData.goalSpawnPoint, Quaternion.identity);


            spawnedGoal.tag = "Goal";


            currentQuestData.ApplyObjectiveBaseOnQuestType(this);
        }
    }

    public void StartQuest(QuestData quest, string sceneName)
    {
        currentQuestData = quest;
        SceneManager.LoadScene(sceneName);
    }

    void SpawnKeys(int amount)
    {
        if (currentQuestData.keyPrefab == null || currentQuestData.keySpawnPoints.Count == 0)
        {
            Debug.LogError("Key Prefab lost");
            return;
        }
        if (currentQuestData.keySpawnPoints == null || currentQuestData.keySpawnPoints.Count == 0)
        {
            Debug.LogError("Key Spawn Points empty.");
            return;
        }
        List<Vector3> availablePoints = new List<Vector3>(currentQuestData.keySpawnPoints);
        for (int i = 0; i < amount; i++)
        {
            if (availablePoints.Count == 0)
            {
                break;
            }
            int randomIndex = Random.Range(0, availablePoints.Count);
            Vector3 spawnPoint = availablePoints[randomIndex];
            Instantiate(currentQuestData.keyPrefab, spawnPoint, Quaternion.identity);
            availablePoints.RemoveAt(randomIndex);
            Debug.Log("Key spawn");

        }

    }

    public void ReachGoal()
    {
        Timer timer = FindFirstObjectByType<Timer>();
        if (timer != null)
        {
            timer.StopTimer();
        }
        currentQuestData.CalculateStarRating(this);

    }
}