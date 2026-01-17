using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public GameObject miniGamePanel;
    private bool isPlayerNear;

    private void Start()
    {
        if (miniGamePanel != null)
            miniGamePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player_Trigger"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player_Trigger"))
        {
            isPlayerNear = false;
            miniGamePanel.SetActive(false);
  
        }
    }

    private void Update()
    {
        if (!isPlayerNear) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            OpenMiniGame();
        }
    }

    private void OpenMiniGame()
    {
        if (miniGamePanel.activeSelf) return;

        miniGamePanel.SetActive(true);
       
    }
}
