using UnityEngine;

public class DialogueEventTrigger : MonoBehaviour
{
    [SerializeField] DialougeeData dialougeeData;
    [SerializeField] bool triggerOnce = true;

    private bool hasTriggered = false;
    public void Trigger()
    {
        if (hasTriggered && triggerOnce)
        {
            return;
        }

        DialougeManager.Instance.StartDialouge(dialougeeData);
        hasTriggered = true;
    }
}
