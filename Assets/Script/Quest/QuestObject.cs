using UnityEngine;

public class QuestObject : MonoBehaviour
{
    bool isplayerinrange;
    GameObject currentMinigame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isplayerinrange && Input.GetKeyDown(KeyCode.F) && currentMinigame == null)
        {
            OpenMinigame();
        }
    }

    private void OpenMinigame()
    {
        Canvas canvas = GameObject.FindAnyObjectByType<Canvas>();
        GameObject prefab = QuestManager.Instance.currentQuestData.minigamePrefab;

        if (prefab != null && canvas != null)
        {
            currentMinigame = Instantiate(prefab, canvas.transform);
            var minigameComponent = currentMinigame.GetComponent<FillingBarMiniGame>();

            minigameComponent.OnComplete += HandleSuccses;
        }
    }

    private void HandleSuccses()
    {
        gameObject.SetActive(false);
        GoalManager.Instance.OnGoalReached(this.gameObject);
        Destroy(gameObject, 0.1f);
        Debug.Log("Minigame completed successfully!");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isplayerinrange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isplayerinrange = false;
    }
}
