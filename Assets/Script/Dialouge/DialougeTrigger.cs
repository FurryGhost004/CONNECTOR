using UnityEngine;

public class DialougeTrigger : MonoBehaviour
{
    [SerializeField] private DialougeeData dialougeeDatas;
    [SerializeField] private bool isTutorialNPC = false;
    bool isNearPlayer = false;



    // Update is called once per frame
    void Update()
    {
        if (!isNearPlayer)
        {
            return;
        }
        if (isNearPlayer && Input.GetKeyDown(KeyCode.F))
        {

            TriggerDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNearPlayer = true;
            Debug.Log("Player is near NPC");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNearPlayer = false;
            Debug.Log("Player left NPC");
        }
    }

    void TriggerDialogue()
    {

        if (isTutorialNPC && TutorialManager.Instance != null)
        {
            if (TutorialSystem.IsCompleted(TutorialType.Finish))
            {
                DialougeManager.Instance.StartDialouge(TutorialManager.Instance.endTutorialDialogue, true);
                return;
            }

            if (TutorialManager.Instance.currentTutorial == TutorialType.Move)
            {
                TutorialManager.Instance.OnMoveTutorialFinish();
            }
            else if (TutorialManager.Instance.currentTutorial == TutorialType.Finish)
            {
                DialougeManager.Instance.StartDialouge(TutorialManager.Instance.endTutorialDialogue, true);
            }
   
        }        
        else
            {
                if (dialougeeDatas != null)
                {
                    DialougeManager.Instance.StartDialouge(dialougeeDatas);
                    Debug.Log("Start Dialouge");
                }
            }

    }


}

