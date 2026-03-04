using System.Collections;
using UnityEngine;

public class DroneEnemy : EnemyBase
{
    
    private float lastAttackTime;

    protected override void UpdateAlertChase()
    {
        if (playerTransform == null) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Nếu đủ gần và hết hồi chiêu thì Dash
        if (distance < 4f && Time.time > lastAttackTime + attackCooldown && !isExecutingAction)
        {
            StartCoroutine(PerformDashAttack());
        }
        else if (!isExecutingAction)
        {
            // Di chuyển bay lượn về phía player
            Vector2 dir = (Vector2)playerTransform.position - rb.position;
            rb.linearVelocity = dir.normalized * chaseSpeed;
        }
    }

    protected override IEnumerator PerformDashAttack()
    {
        isExecutingAction = true;
        currentState = State.Dashing;
        hasDealtDamageThisDash = false;

        // 1. Gồng (Wind-up)
        rb.linearVelocity = Vector2.zero;
        animator.SetBool(hashPreparing, true);
        yield return new WaitForSeconds(windUpTime);

        // 2. Lao tới (Dash)
        animator.SetBool(hashPreparing, false);
        animator.SetTrigger(hashAttack);
        Vector2 dashDir = (playerTransform.position - transform.position).normalized;
        rb.linearVelocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // 3. Hồi phục
        currentState = State.Recovering;
        rb.linearVelocity = Vector2.zero;
        lastAttackTime = Time.time;
        yield return new WaitForSeconds(1f);

        isExecutingAction = false;
        currentState = State.Alert;
    }

}
