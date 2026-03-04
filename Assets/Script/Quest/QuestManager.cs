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
    private QuestData lastFinishedQuest;


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
            CheckAndProgressLevel();

            // 2. Refresh NPC
            if (NPCManager.Instance != null)
            {
                NPCManager.Instance.RefreshNPCs();
            }

            // 3. Hiển thị Dialogue nếu có (Sử dụng hệ thống đánh dấu đã nói ở phần 1)
            CheckForEndMissionDialogue();

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
        yield return new WaitForSeconds(0.1f);

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
        if (currentQuestData.goalPrefab != null && currentQuestData.goalSpawnPoint != null)
        {

            foreach (Vector3 spawnPos in currentQuestData.goalSpawnPoint)
            {
                GameObject spawnedGoal = Instantiate(currentQuestData.goalPrefab, spawnPos, Quaternion.identity);
                if (spawnedGoal.GetComponent<Goal>() == null)
                {
                    spawnedGoal.AddComponent<Goal>();
                }
                spawnedGoal.transform.SetParent(null);
                SceneManager.MoveGameObjectToScene(spawnedGoal, SceneManager.GetActiveScene());
                Debug.Log("Goal spawn");
            }

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
        if (currentQuestData.keySpawnPoints == null || currentQuestData.keySpawnPoints.Count == 0)
        {
            Debug.LogError("Key Spawn Points empty.");
            return;
        }
        if (currentQuestData.keyPrefab == null)
        {
            Debug.LogError("Key Prefab is missing!");
            return;
        }
        List<Vector3> availablePoints = new List<Vector3>(currentQuestData.keySpawnPoints);
        int spawnAmount = Mathf.Min(amount, availablePoints.Count);
        for (int i = 0; i < spawnAmount; i++)
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
        Debug.Log("Goal Reached,StopTime!");
        lastFinishedQuest = currentQuestData;
        currentQuestData.CalculateStarRating(this);

    }
    void CheckAndProgressLevel()
    {

        int highestIndex = PlayerPrefs.GetInt("HighestMissionIndex", 0);
        int currentMaxLevel = PlayerPrefs.GetInt("CurrentMaxLevel", 1);

        if (highestIndex >= 10 && currentMaxLevel < 3)
        {
            PlayerPrefs.SetInt("CurrentMaxLevel", 3);
        }
        else if (highestIndex >= 5 && currentMaxLevel < 2)
        {
            PlayerPrefs.SetInt("CurrentMaxLevel", 2);
        }

        PlayerPrefs.Save();
    }
    void CheckForEndMissionDialogue()
    {
        // 1. Kiểm tra xem có Quest nào vừa mới kết thúc không
        if (lastFinishedQuest == null) return;

        // 2. Kiểm tra xem Quest đó có dữ liệu hội thoại không
        if (lastFinishedQuest.completionDialogue == null)
        {
            lastFinishedQuest = null; // Reset để không check lại lần sau
            return;
        }

        string dialogueKey = "Dialogue_Mission_" + lastFinishedQuest.missionIndex + "_Shown";
        bool hasShownDialogue = PlayerPrefs.GetInt(dialogueKey, 0) == 1;

        if (!hasShownDialogue)
        {
            // 3. Sử dụng lastFinishedQuest để lấy dialogue
            DialougeManager.Instance.StartDialouge(lastFinishedQuest.completionDialogue);

            // 4. Đánh dấu đã xem
            PlayerPrefs.SetInt(dialogueKey, 1);
            PlayerPrefs.Save();

            Debug.Log($"Showing completion dialogue for Mission: {lastFinishedQuest.missionIndex}");
        }

        // Quan trọng: Xóa dấu vết quest cũ để khi chuyển cảnh tiếp theo không hiện lại
        lastFinishedQuest = null;
    }
}
