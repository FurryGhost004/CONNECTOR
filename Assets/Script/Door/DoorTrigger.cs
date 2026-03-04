using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject miniGamePanel;
    private bool isPlayerNear;

    private void Start()
    {
        miniGamePanel.SetActive(false);

        // đảm bảo singleton tồn tại
        if (UnlockManifolds.instance == null)
        {
            UnlockManifolds.instance = FindObjectOfType<UnlockManifolds>();
        }
    }

    private void Update()
    {
        if (!isPlayerNear) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            UnlockManifolds.currentDoor = gameObject;
            miniGamePanel.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player_Trigger")) return;

        isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player_Trigger")) return;

        isPlayerNear = false;
        miniGamePanel.SetActive(false);

        if (UnlockManifolds.instance != null)
        {
            UnlockManifolds.instance.RestartTheGame();
        }
    }
}
