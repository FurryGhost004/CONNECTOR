using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
public class GroundEnemyController : MonoBehaviour
{
    private enum State { Patrol, Alert, Dashing, Recovering }
    [SerializeField] private State currentState = State.Patrol;

    [Header("Movement Settings")]
    public float walkSpeed = 2.5f;
    public float chaseSpeed = 5f;
    public float wanderRadius = 4f;

    [Header("Hover Effect")]
    public float hoverAmplitude = 0.3f;
    public float hoverFrequency = 2f;

    [Header("Attack Settings")]
    public float damage = 10f;
    public float dashForce = 18f;
    public float windUpTime = 0.7f; // Thời gian đứng yên gồng
    public float attackCooldown = 2f;

    [Header("Detection & Lost Logic")]
    public VisionCone visionCone;
    public float timeToLostPlayer = 2f;

    private float lostTimer = 0f;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform playerTransform;
    private Vector2 patrolCenter;
    private Vector2 targetPatrolPos;
    private bool isExecutingAction = false;
    private bool hasDealtDamageThisDash = false;

    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashAttack = Animator.StringToHash("Attack");
    private readonly int hashPreparing = Animator.StringToHash("IsPreparing");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        patrolCenter = transform.position;
        targetPatrolPos = GetRandomPoint();
    }

    void Start()
    {
        if (visionCone != null) visionCone.OnPlayerSpotted += HandleDiscovery;
    }

    void Update()
    {
        if (isExecutingAction) return;

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                UpdateFacing(rb.linearVelocity);
                break;
            case State.Alert:
                UpdateChase();
                LookAtPlayer();
                CheckLostPlayer();
                break;
        }

        animator.SetFloat(hashSpeed, rb.linearVelocity.magnitude);
    }

    #region AI Logic
    private void UpdatePatrol()
    {
        Vector2 dir = (targetPatrolPos - (Vector2)transform.position).normalized;
        float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

        rb.linearVelocity = dir * walkSpeed + Vector2.up * hover;

        if (Vector2.Distance(transform.position, targetPatrolPos) < 0.5f)
            targetPatrolPos = GetRandomPoint();
    }

    private void UpdateChase()
    {
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist < 5f && visionCone.canSeePlayer)
            StartCoroutine(PerformDashAttack());
        else
        {
            Vector2 dir = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, dir * chaseSpeed, Time.deltaTime * 2f);
        }
    }

    private IEnumerator PerformDashAttack()
    {
        isExecutingAction = true;
        currentState = State.Dashing;
        hasDealtDamageThisDash = false;

        // Bắt đầu trạng thái chuẩn bị (Gồng)
        animator.SetBool(hashPreparing, true);

        // 1. Đứng yên gồng (Wind-up) tại chỗ
        rb.linearVelocity = Vector2.zero;

        // Rung nhẹ tại chỗ để báo hiệu sắp lao tới (tùy chọn)
        float t = 0;
        while (t < windUpTime)
        {
            LookAtPlayer();
            // transform.position += (Vector3)UnityEngine.Random.insideUnitCircle * 0.05f; // Bỏ comment nếu muốn rung
            t += Time.deltaTime;
            yield return null;
        }

        // 2. Dash thẳng tới mục tiêu
        if (playerTransform != null)
        {
            animator.SetBool(hashPreparing, false);
            animator.SetTrigger(hashAttack);

            Vector2 dashDir = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.linearVelocity = dashDir * dashForce;

            yield return new WaitForSeconds(0.6f);
        }

        // 3. Recovery (Nghỉ ngơi sau khi lao)
        rb.linearVelocity = Vector2.zero;
        currentState = State.Recovering;
        yield return new WaitForSeconds(attackCooldown);

        isExecutingAction = false;
        currentState = State.Alert;
    }

    private void CheckLostPlayer()
    {
        if (visionCone != null && !visionCone.canSeePlayer)
        {
            lostTimer += Time.deltaTime;
            if (lostTimer >= timeToLostPlayer)
            {
                currentState = State.Patrol;
                playerTransform = null;
            }
        }
        else lostTimer = 0f;
    }
    #endregion

    #region Collision & Damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState == State.Dashing && !hasDealtDamageThisDash)
            {
                ApplyDamageToPlayer(collision.gameObject, damage);
                hasDealtDamageThisDash = true;
                rb.linearVelocity = -rb.linearVelocity * 0.2f;
            }
            else if (currentState != State.Dashing && currentState != State.Recovering)
            {
                ApplyDamageToPlayer(collision.gameObject, damage * 0.5f);
            }
        }
    }

    private void ApplyDamageToPlayer(GameObject player, float amount)
    {
        if (HealthSystem.Instance != null)
        {
            HealthSystem.Instance.TakeDamage(amount, transform.position);
            Debug.Log("Gây " + amount + " dame!");
        }
    }
    #endregion

    #region Helpers
    private void LookAtPlayer()
    {
        if (playerTransform == null) return;
        transform.localScale = new Vector3((playerTransform.position.x < transform.position.x) ? -1f : 1f, 1, 1);
    }

    private void UpdateFacing(Vector2 moveDir)
    {
        if (Mathf.Abs(moveDir.x) > 0.1f)
            transform.localScale = new Vector3((moveDir.x < 0) ? -1f : 1f, 1, 1);
    }

    private Vector2 GetRandomPoint() => patrolCenter + UnityEngine.Random.insideUnitCircle * wanderRadius;

    private void HandleDiscovery(Transform player)
    {
        playerTransform = player;
        if (currentState == State.Patrol) currentState = State.Alert;
    }
    #endregion
}