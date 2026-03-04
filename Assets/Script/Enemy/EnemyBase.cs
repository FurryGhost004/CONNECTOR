using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public abstract class EnemyBase : MonoBehaviour, ITauntable
{
    protected enum State { Patrol, Alert, Dashing, Recovering, Stunned, Slowed, Taunted }
    [SerializeField] protected State currentState = State.Patrol;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float originalWalkSpeed;
    public float originalChaseSpeed;
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;
    public float wanderRadius = 4f;

    [Header("Attack Settings")]
    public float dashSpeed = 22f;
    public float windUpTime = 0.8f;
    public float dashDuration = 0.7f;
    public float attackCooldown = 2.0f;

    [Header("Detection Logic")]
    public VisionCone visionCone;
    public float damage = 15f;
    public float timeToLostPlayer = 2f;

    // Biến dùng chung cho cả 2 loại quái
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform playerTransform;
    protected Vector2 targetPos;
    protected Vector2 patrolCenter;
    protected float lostTimer = 0f;
    protected bool isExecutingAction = false;
    protected bool hasDealtDamageThisDash = false;
    protected Vector3 tauntPosition;
    protected float tauntTimer = 0f;

    // Hashes cho Animator
    protected readonly int hashSpeed = Animator.StringToHash("Speed");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashPreparing = Animator.StringToHash("IsPreparing");
    protected bool hasPlayedAlertSFX = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        patrolCenter = transform.position;
    }

    protected virtual void Start()
    {
        if (visionCone != null) visionCone.OnPlayerSpotted += HandleDiscovery;
        targetPos = GetRandomPoint();
    }

    protected virtual void Update()
    {
        if (isExecutingAction) return;

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                UpdateFacing(rb.linearVelocity);
                break;
            case State.Alert:
                UpdateAlertChase();
                LookAtPlayer();
                CheckLostPlayer();
                break;
            case State.Taunted:
                UpdateTaunted();
                break;
        }
        animator.SetFloat(hashSpeed, rb.linearVelocity.magnitude);
    }

    // Logic tuần tra chung (bay lượn/di chuyển tự do)
    protected virtual void UpdatePatrol()
    {
        Vector2 direction = (targetPos - rb.position).normalized;
        float hover = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        rb.linearVelocity = direction * patrolSpeed + Vector2.up * hover;

        if (Vector2.Distance(rb.position, targetPos) < 0.5f)
            targetPos = GetRandomPoint();
    }

    protected void CheckLostPlayer()
    {
        if (visionCone != null && !visionCone.canSeePlayer)
        {
            lostTimer += Time.deltaTime;

            if (lostTimer >= timeToLostPlayer)
            {
                currentState = State.Patrol;
                playerTransform = null;
                hasPlayedAlertSFX = false; // ✅ reset để lần sau kêu lại
            }
        }
        else
        {
            lostTimer = 0f;
        }
    }


    protected void LookAtPlayer()
    {
        if (playerTransform == null) return;
        transform.localScale = new Vector3((playerTransform.position.x < transform.position.x) ? -1f : 1f, 1, 1);
    }

    protected void UpdateFacing(Vector2 moveDir)
    {
        if (Mathf.Abs(moveDir.x) > 0.1f)
            transform.localScale = new Vector3((moveDir.x < 0) ? -1f : 1f, 1, 1);
    }

    protected void HandleDiscovery(Transform player)
    {
        playerTransform = player;

        if (currentState == State.Patrol)
        {
            currentState = State.Alert;

            // 🔊 CHỈ PHÁT 1 LẦN
            if (!hasPlayedAlertSFX)
            {
                EnemyAudio audio = GetComponent<EnemyAudio>();
                if (audio != null)
                    audio.PlayAlertSFX();

                hasPlayedAlertSFX = true;
            }
        }
    }


    protected Vector2 GetRandomPoint() => patrolCenter + (Vector2)UnityEngine.Random.insideUnitCircle * wanderRadius;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentState == State.Dashing && !hasDealtDamageThisDash)
            {
                if (HealthSystem.Instance != null) HealthSystem.Instance.TakeDamage(damage, transform.position);
                hasDealtDamageThisDash = true;
                rb.linearVelocity = -rb.linearVelocity * 0.3f;
            }
        }
    }

    // Hàm trừ máu chung (sử dụng logic nãy bạn đưa)
    protected void ApplyDamage(float amount)
    {
        if (HealthSystem.Instance != null) HealthSystem.Instance.TakeDamage(amount, transform.position);
    }
    public void ApplyTaunt(float duration, Vector3 position)
    {

        StopAllCoroutines();


        isExecutingAction = false;

        tauntPosition = position;
        tauntTimer = duration;
        currentState = State.Taunted;

        Debug.Log($"{gameObject.name} is taunted towards {tauntPosition} for {duration} seconds.");
    }

    // Mỗi con sẽ có cách Dash khác nhau nên để abstract/virtual
    protected abstract void UpdateAlertChase();
    protected abstract IEnumerator PerformDashAttack();
    protected void PickNewPatrolTarget()
    {
        targetPos = patrolCenter + UnityEngine.Random.insideUnitCircle * wanderRadius;
    }
    protected virtual void UpdateTaunted()
    {
        if (tauntTimer > 0)
        {
            tauntTimer -= Time.deltaTime;


            Vector2 dir = (Vector2)tauntPosition - rb.position;
            float distance = dir.magnitude;

            if (distance > 0.5f)
            {

                rb.linearVelocity = dir.normalized * chaseSpeed; 
                UpdateFacing(dir);
            }
            else
            {

                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {

            currentState = State.Patrol;
            targetPos = GetRandomPoint(); 
        }
    }
}