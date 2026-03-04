using UnityEngine;

public class GoalBoss : MonoBehaviour
{
    [SerializeField] GameObject Goal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Goal.SetActive(true);
            
        }
    }
}
