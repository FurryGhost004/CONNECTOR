using UnityEngine;
using System.Collections;

public class MultiLaserController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Prefabs")]
    public LineRenderer warningPrefab;
    public LineRenderer laserPrefab;

    [Header("Laser Settings")]
    public int laserCount = 5;
    public float spacing = 1.2f;
    public float laserLength = 30f;
    public float warningTime = 1.2f;
    public float laserTime = 1.5f;

    [Header("Movement Settings")]
    public float followSpeed = 6f;
    public float alignDistance = 0.1f;

    private bool isAttacking = false;
    private EnemyAudio enemyAudio;

    void Awake()
    {
        enemyAudio = GetComponent<EnemyAudio>();
    }

    // ==================================================
    // GỌI TỪ BOSS CONTROLLER
    // ==================================================
    public void FireFromTop()
    {
        if (!isAttacking)
            StartCoroutine(MoveAndFireRoutine());
    }

    // ==================================================
    // DI CHUYỂN THEO X PLAYER → BẮN XUỐNG
    // ==================================================
    IEnumerator MoveAndFireRoutine()
    {
        if (player == null) yield break;

        isAttacking = true;

        float targetX = player.position.x;

        // 1️⃣ Trượt ngang theo player
        while (Mathf.Abs(transform.position.x - targetX) > alignDistance)
        {
            Vector3 pos = transform.position;

            pos.x = Mathf.MoveTowards(
                transform.position.x,
                targetX,
                followSpeed * Time.deltaTime
            );

            transform.position = pos;
            yield return null;
        }

        // 2️⃣ Bắn xuống dưới
        yield return StartCoroutine(MultiLaserRoutine(Vector2.down));

        isAttacking = false;
    }

    // ==================================================
    // BẮN CỤM LASER
    // ==================================================
    IEnumerator MultiLaserRoutine(Vector2 dir)
    {
        Vector2 perpendicular = new Vector2(-dir.y, dir.x);

        LineRenderer[] warnings = new LineRenderer[laserCount];
        LineRenderer[] lasers = new LineRenderer[laserCount];

        float center = (laserCount - 1) / 2f;

        // 🔊 Âm thanh charge laser
        if (enemyAudio != null)
            enemyAudio.PlayLaserCharge();

        // 1️⃣ Spawn warning
        for (int i = 0; i < laserCount; i++)
        {
            float offset = (i - center) * spacing;

            Vector3 start = transform.position + (Vector3)(perpendicular * offset);
            Vector3 end = start + (Vector3)(dir * laserLength);

            LineRenderer w = Instantiate(warningPrefab);
            w.enabled = true;
            w.SetPosition(0, start);
            w.SetPosition(1, end);

            warnings[i] = w;
        }

        yield return new WaitForSeconds(warningTime);

        // 🔊 Âm thanh laser fire
        if (enemyAudio != null)
            enemyAudio.PlayLaserFire();

        // 2️⃣ Spawn laser thật
        for (int i = 0; i < laserCount; i++)
        {
            if (warnings[i] != null)
                Destroy(warnings[i].gameObject);

            float offset = (i - center) * spacing;

            Vector3 start = transform.position + (Vector3)(perpendicular * offset);
            Vector3 end = start + (Vector3)(dir * laserLength);

            LineRenderer l = Instantiate(laserPrefab);
            l.enabled = true;
            l.SetPosition(0, start);
            l.SetPosition(1, end);

            LaserDamage damageScript = l.gameObject.AddComponent<LaserDamage>();
            damageScript.Setup(start, dir, laserLength);

            lasers[i] = l;
        }

        yield return new WaitForSeconds(laserTime);

        for (int i = 0; i < laserCount; i++)
        {
            if (lasers[i] != null)
                Destroy(lasers[i].gameObject);
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}