using UnityEngine;
using System.Collections;

public class WarningCircle : MonoBehaviour
{
    public float warningTime = 3f;
    public float damage = 20f;

    private bool playerInside = false;
    private Transform player;

    public System.Action OnStrike; // gọi sét

    void Start()
    {
        StartCoroutine(WarningRoutine());
    }

    IEnumerator WarningRoutine()
    {
        yield return new WaitForSeconds(warningTime);

        // Nếu player còn đứng trong vòng → trừ máu
        if (playerInside && player != null)
        {
            HealthSystem.Instance.TakeDamage(damage, transform.position);
        }

        // Gọi sấm sét (animation)
        OnStrike?.Invoke();

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
