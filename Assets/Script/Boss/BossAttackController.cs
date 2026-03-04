using UnityEngine;
using System.Collections;

public class BossAttackController : MonoBehaviour
{
    [Header("Attack References")]
    public BossMultiLightningAttack lightningAttack;
    public MultiLaserController laserAttack;

    [Header("Attack Settings")]
    public float attackInterval = 4f;
    public float startDelay = 2f;

    private Coroutine attackCoroutine;
    private bool isDead = false;

    void Start()
    {
        attackCoroutine = StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(startDelay);

        while (!isDead)
        {
            PerformRandomAttack();
            yield return new WaitForSeconds(attackInterval);
        }
    }

    void PerformRandomAttack()
    {
        if (isDead) return;

        int random = Random.Range(0, 2);

        switch (random)
        {
            case 0:
                Debug.Log("Boss dùng Lightning!");
                lightningAttack.CastMultiLightning();
                break;

            case 1:
                Debug.Log("Boss dùng Laser!");
                laserAttack.FireFromTop();
                break;
        }
    }

    public void SetAttackInterval(float newInterval)
    {
        attackInterval = newInterval;
    }

    // ==================================================
    // 🔥 GỌI TỪ BossHealth
    // ==================================================
    public void HandleBossDeath(BossHealth bossHealth)
    {
        if (isDead) return;

        isDead = true;

        // Dừng loop attack mới
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        StartCoroutine(WaitForAttacksThenDie(bossHealth));
    }

    IEnumerator WaitForAttacksThenDie(BossHealth bossHealth)
    {
        // Chờ laser nếu đang bắn
        if (laserAttack != null)
        {
            while (laserAttack.IsAttacking())
                yield return null;
        }

        // Chờ lightning nếu đang đánh
        if (lightningAttack != null)
        {
            while (lightningAttack.IsAttacking())
                yield return null;
        }

        // Sau khi xong hết → báo lại BossHealth
        bossHealth.FinalDeath();
    }
}