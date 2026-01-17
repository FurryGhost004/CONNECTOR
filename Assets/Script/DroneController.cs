using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class DroneController : EnemyBase
{
    [Header("Drone Specific Positioning")]
    public float hoverAboveHeight = 4f;
    public float horizontalOffset = 2.5f;
    public float prepareMoveSpeed = 10f;

    [Header("After Image Settings")]
    public bool useAfterImage = true;
    public float ghostDelay = 0.05f;
    public Color ghostColor = new Color(0.5f, 0.8f, 1f, 0.5f);
    public float ghostFadeSpeed = 3f;

    private SpriteRenderer droneSR;
    private Collider2D droneCollider;
    private EnemyAudio enemyAudio;
    


    protected override void Start()
    {
        base.Start();
        droneSR = GetComponent<SpriteRenderer>();
        droneCollider = GetComponent<Collider2D>();

        // Mặc định Drone bay xuyên thấu (Trigger)
        if (droneCollider != null) droneCollider.isTrigger = true;
    }

    protected override void Awake()
    {
        base.Awake(); // ⭐ RẤT QUAN TRỌNG

        enemyAudio = GetComponent<EnemyAudio>();
        if (enemyAudio == null)
        {
            Debug.LogError("❌ EnemyAudio chưa được gắn vào Drone!", this);
        }
    }

    protected override void Update()
    {
        base.Update();
        UpdateVisionConeColor();
    }

    private void UpdateVisionConeColor()
    {
        if (visionCone == null) return;

        bool shouldBeAlert = (currentState == State.Alert ||
                             currentState == State.Dashing ||
                             currentState == State.Recovering);

        visionCone.SetAlertMode(shouldBeAlert);
    }

    // ==========================================
    // ANIMATION EVENTS
    // ==========================================

    // 1. Cho Idle/Run (Fly): Không collider, dùng cho hiệu ứng/âm thanh
    public void OnDroneFlightEffect(string effectType)
    {
        // Ví dụ: Phát sound bay, tạo bụi...
        // Debug.Log("Drone Effect: " + effectType);
    }

    // 2. Cho Attack: Điều khiển Collider (1 = Đặc/Solid, 0 = Xuyên thấu/Trigger)
    public void OnDashAttackCollider(int isSolid)
    {
        if (droneCollider == null) return;

        // Khi tấn công (isSolid = 1) thì isTrigger = false (để có va chạm vật lý)
        droneCollider.isTrigger = (isSolid == 0);
    }

    protected override void UpdateAlertChase()
    {
        if (playerTransform == null || isExecutingAction) return;

        float dist = Vector2.Distance(rb.position, playerTransform.position);

        // 👉 Khi PHÁT HIỆN PLAYER
        if (dist < 8f && visionCone.canSeePlayer)
        {
            // 🔊 CHỈ PHÁT 1 LẦN
            if (!hasPlayedAlertSFX)
            {
                enemyAudio.PlayAlertSFX();
                hasPlayedAlertSFX = true;
            }

            StartCoroutine(PerformDashAttack());
        }
        else
        {
            Vector2 moveDir = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                moveDir * chaseSpeed,
                Time.deltaTime * 2f
            );

            UpdateFacing(rb.linearVelocity);
        }
    }



    protected override IEnumerator PerformDashAttack()
    {
        isExecutingAction = true;
        hasDealtDamageThisDash = false;
        animator.SetBool(hashPreparing, true);

        // 1. Bay lên vị trí chuẩn bị
        float approachTimer = 0;
        enemyAudio.PlayAttackEffect(); // Phát âm thanh tấn công
        while (approachTimer < 2.0f)
        {
            if (playerTransform == null || currentState == State.Stunned) break;

            float side = (playerTransform.position.x > transform.position.x) ? -horizontalOffset : horizontalOffset;
            Vector2 preparePos = (Vector2)playerTransform.position + new Vector2(side, hoverAboveHeight);

            if (Vector2.Distance(rb.position, preparePos) < 0.4f) break;

            rb.linearVelocity = (preparePos - rb.position).normalized * prepareMoveSpeed;
            LookAtPlayer();
            approachTimer += Time.deltaTime;
            yield return null;
        }

        // 2. Gồng nhắm (Rung rinh)
        rb.linearVelocity = Vector2.zero;
        float t = 0;
        while (t < windUpTime)
        {
            if (currentState == State.Stunned) yield break;
            transform.position += (Vector3)UnityEngine.Random.insideUnitCircle * 0.08f;
            t += Time.deltaTime;
            yield return null;
        }

        // 3. Dash (Tấn công)
        if (playerTransform != null && currentState != State.Stunned)
        {
            animator.SetBool(hashPreparing, false);
            animator.SetTrigger(hashAttack); // Gắn Animation Event OnDashAttackCollider(1) vào clip này
            currentState = State.Dashing;

            if (useAfterImage) StartCoroutine(CreateAfterImageRoutine());

            Vector2 attackDir = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.linearVelocity = attackDir * dashSpeed;
            yield return new WaitForSeconds(dashDuration);
        }

        // 4. Recovery
        rb.linearVelocity = Vector2.zero;

        // Safety check: Luôn trả về xuyên thấu sau khi dash xong
        if (droneCollider != null) droneCollider.isTrigger = true;

        if (currentState != State.Stunned) currentState = State.Recovering;
        yield return new WaitForSeconds(attackCooldown);

        isExecutingAction = false;
        if (currentState != State.Stunned) currentState = State.Alert;
    }

    private IEnumerator CreateAfterImageRoutine()
    {
        while (currentState == State.Dashing)
        {
            GameObject ghost = new GameObject("Drone_Ghost");
            AfterImage script = ghost.AddComponent<AfterImage>();

            script.Init(
                droneSR.sprite,
                transform.position,
                transform.rotation,
                transform.localScale,
                ghostColor,
                ghostFadeSpeed,
                droneSR.sortingOrder
            );

            yield return new WaitForSeconds(ghostDelay);
        }
    }

    #region Status Effects
    public void SetStunned(bool value)
    {
        if (value)
        {
            hasPlayedAlertSFX = false;
            currentState = State.Stunned;
            rb.linearVelocity = Vector2.zero;
            StopAllCoroutines();
            isExecutingAction = false;
            animator.SetBool(hashPreparing, false);
            if (droneCollider != null) droneCollider.isTrigger = true; // Stun thì xuyên thấu để ko kẹt
        }
        else
        {
            currentState = State.Patrol;
            PickNewPatrolTarget();
        }
    }

    public void StunDuration(float duration) => StartCoroutine(StunRoutine(duration));
    private IEnumerator StunRoutine(float duration)
    {
        SetStunned(true);
        yield return new WaitForSeconds(duration);
        SetStunned(false);
    }

    public void SetSlowed(float multiplier)
    {
        chaseSpeed *= multiplier;
        patrolSpeed *= multiplier;
    }

    public void ResetSpeed()
    {
        patrolSpeed = originalWalkSpeed;
        chaseSpeed = originalChaseSpeed;
    }

    public void SlowDuration(float duration, float multiplier) => StartCoroutine(SlowRoutine(duration, multiplier));
    private IEnumerator SlowRoutine(float duration, float multiplier)
    {
        SetSlowed(multiplier);
        yield return new WaitForSeconds(duration);
        ResetSpeed();
    }

    public void SetTaunt(bool value, Vector3 tauntTarget)
    {
        if (value)
        {
            hasPlayedAlertSFX = false;
            currentState = State.Taunted;
            rb.linearVelocity = Vector2.zero;
            tauntPosition = tauntTarget;
            StopAllCoroutines();
            isExecutingAction = false;
        }
        else
        {
            currentState = State.Patrol;
            PickNewPatrolTarget();
        }
    }

    public void TauntDuration(float duration, Vector3 tauntTarget) => StartCoroutine(TauntRoutine(duration, tauntTarget));
    private IEnumerator TauntRoutine(float duration, Vector3 tauntTarget)
    {
        SetTaunt(true, tauntTarget);
        yield return new WaitForSeconds(duration);
        SetTaunt(false, tauntTarget);
    }
    #endregion
}