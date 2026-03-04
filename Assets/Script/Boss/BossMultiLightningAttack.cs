using UnityEngine;
using System.Collections;

public class BossMultiLightningAttack : MonoBehaviour
{
    public GameObject warningCirclePrefab;
    public GameObject lightningPrefab;
    public Transform player;

    public int lightningCount = 5;
    public float distanceBetweenLightnings = 1.5f;

    private int activeStrikes = 0;
    private bool isDying = false;
    private EnemyAudio enemyAudio;

    void Awake()
    {
        enemyAudio = GetComponent<EnemyAudio>();
    }

    // ==================================================
    // GỌI TỪ BOSS CONTROLLER
    // ==================================================
    public void CastMultiLightning()
    {
        if (player == null || isDying) return;

        // Sét đầu tiên đánh đúng player
        SpawnWarningAndLightning(player.position);

        // Các sét còn lại xung quanh
        for (int i = 1; i < lightningCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized
                             * distanceBetweenLightnings * i;

            SpawnWarningAndLightning(player.position + (Vector3)offset);
        }
    }

    void SpawnWarningAndLightning(Vector3 position)
    {
        activeStrikes++;

        // 🔊 PHÁT ÂM THANH CẢNH BÁO
        if (enemyAudio != null)
            enemyAudio.PlayLightningWarning(position);

        GameObject warning = Instantiate(
            warningCirclePrefab,
            position,
            Quaternion.identity
        );

        WarningCircle wc = warning.GetComponent<WarningCircle>();

        wc.OnStrike = () =>
        {
            Vector3 correctedPos = position + Vector3.up * 4f;

            // 🔊 PHÁT ÂM THANH SÉT ĐÁNH
            if (enemyAudio != null)
                enemyAudio.PlayLightningStrike(correctedPos);

            GameObject lightning = Instantiate(
                lightningPrefab,
                correctedPos,
                Quaternion.identity
            );

            StartCoroutine(LightningLifeRoutine(lightning));
        };
    }

    IEnumerator LightningLifeRoutine(GameObject lightning)
    {
        yield return new WaitForSeconds(1f); // thời gian tồn tại

        if (lightning != null)
            Destroy(lightning);

        activeStrikes--;

        // Nếu boss đang chết và đã hết strike → xóa boss
        if (isDying && activeStrikes <= 0)
        {
            Destroy(gameObject);
        }
    }

    // ==================================================
    // GỌI KHI BOSS CHẾT
    // ==================================================
    public void OnBossDeath()
    {
        isDying = true;

        // Nếu không có strike nào đang chạy → chết ngay
        if (activeStrikes <= 0)
        {
            Destroy(gameObject);
        }
    }

    // ==================================================
    // CHO BossAttackController CHECK
    // ==================================================
    public bool IsAttacking()
    {
        return activeStrikes > 0;
    }
}