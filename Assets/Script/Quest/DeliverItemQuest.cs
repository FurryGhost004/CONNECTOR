using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeliverItemQuest", menuName = "Quests/DeliverItemQuest")]
public class DeliverItemQuest : QuestData
{

    public int keyItemAmount;
    public List<Item> itemsToDeliver;
    public override void ApplyObjectiveBaseOnQuestType(QuestManager questManager)
    {
        var inventory = InventoryManager.Instance;
        if (inventory == null) return;
        List<Item> currentItems = inventory.GetItemsList();
        int spaceNeeded = itemsToDeliver.Count;
        int currentCount = currentItems.Count;
        int maxSlots = inventory.maxSlots;

        int amountToRemove = (currentCount + spaceNeeded) - maxSlots;

        if (amountToRemove > 0)
        {
            for (int i = 0; i < amountToRemove; i++)
            {
                int lastIndex = currentItems.Count - 1;
                if (lastIndex < 0) break;

                Item itemToRemove = currentItems[lastIndex];


                int currentMoney = PlayerPrefs.GetInt("Money", 0);
                PlayerPrefs.SetInt("Money", currentMoney + itemToRemove.price);
                Debug.Log($"{itemToRemove.name} đã bị loại bỏ khỏi kho để nhường chỗ cho Quest Item. Bạn nhận lại: {itemToRemove.price} $.");
                PlayerPrefs.Save();


                Debug.Log($"[Quest] Giải phóng ô thứ {lastIndex} để lấy chỗ cho Quest Item. Hoàn lại: {itemToRemove.price}");

                currentItems.RemoveAt(lastIndex);
            }
        }

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
