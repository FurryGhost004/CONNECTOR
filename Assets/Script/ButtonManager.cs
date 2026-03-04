using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    Item item;
    [SerializeField] GameObject questPanel;
    [SerializeField] GameObject shopclose;
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Return()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.currentQuestData = null;
        }
        SceneManager.LoadScene("WaittingRoom");
    }

    public void CloseQuest()
    {
        questPanel.SetActive(false);
    }
    public void AcceptQuest1()
    {
        SceneManager.LoadScene("Model_1");
    }

    public void Buy()
    {
        if (ShopManager.Instance.CurrentItem != null)
        {
            ShopManager.Instance.Buy();
        }
    }
    public void CloseShop()
        {
        shopclose.SetActive(false);
    }
    public void StartSelectedMission()
    {
        if (MiissionButtonManager.SelectedQuest != null)
        {
            QuestManager.Instance.StartQuest(
                MiissionButtonManager.SelectedQuest,
                MiissionButtonManager.SelectedScene
            );
        }
        else
        {
            Debug.LogWarning("No mission selected!");
        }
    }

    public void BossQuest1()
    {
        SceneManager.LoadScene("Boss");
    }
}
