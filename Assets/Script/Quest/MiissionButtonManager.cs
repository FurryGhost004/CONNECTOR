using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiissionButtonManager : MonoBehaviour
{
    [SerializeField] private QuestData questData;
    [SerializeField] string targetScene;

    [Header("Info Panel References")]
    public TextMeshProUGUI QuestNameDisplay;
    public TextMeshProUGUI QuestDescriptionDisplay;
    public TextMeshProUGUI QuestTypeDisplay;
    public TextMeshProUGUI QuestReward;
    public TextMeshProUGUI QuestDifficultyDisplay;
    public TextMeshProUGUI QuestBestTimeDisplay;

    public static QuestData SelectedQuest;
    public static string SelectedScene;
    public void SetupMissionButton(QuestData data)
    {
        questData = data;
    }
    public void OnMissionButtonClicked()
    {
        if (questData == null)
        {
            Debug.LogError("Quest Data is missing on this button!");
            return;
        }

        SelectedQuest = questData;
        SelectedScene = targetScene;
        UpdateUI();



    }

    void UpdateUI()
    {
        QuestNameDisplay.text = questData.questName;
        QuestDescriptionDisplay.text = questData.questDescription;
        QuestTypeDisplay.text = "Type: " + questData.questType.ToString();
        QuestDifficultyDisplay.text = "Difficulty: " + questData.difficultyDescription;
        QuestReward.text = "Reward: " + questData.rewardPerStar.ToString() + " Per Star";

        float bestTime = PlayerPrefs.GetFloat(questData.questName + "_BestTime", 0);
        if (bestTime > 0)
        {
            int mins = Mathf.FloorToInt(bestTime / 60);
            int secs = Mathf.FloorToInt(bestTime % 60);
            QuestBestTimeDisplay.text = string.Format("Best Time: {0:00}:{1:00}", mins, secs);
        }
        else
        {
            QuestBestTimeDisplay.text = "Best Time: No Record";
        }

    }

}
