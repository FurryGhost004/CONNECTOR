using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour, ITauntable
{
    
    [SerializeField] protected State currentState = State.Patrol;
    // =====================
    // MOVEMENT
    // =====================
    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 5f;

    // =====================
    // PATROL
    // =====================
    [Header("Patrol")]
    public float patrolRadius = 3f;
    public float patrolSegmentTime = 3f;
    public float patrolStopTime = 1f;
    public LayerMask obstacleLayer;

    // =====================
    // ATTACK
    // =====================
    [Header("Attack")]
    public float attackDistance = 0.8f;
    public float attackReleaseDistance = 1.2f;
    public float attackCooldown = 1.2f;

    // =====================
    // EFFECT
    // =====================
    [Header("Effect")]
    public float slowMultiplier = 0.5f;

    // =====================
    // LOST PLAYER
    // =====================
    [Header("Lost Player")]
    [SerializeField] protected float chaseGraceDuration = 3f;
    [SerializeField] protected float idleAfterLostDuration = 3f;

    // =====================
    // LEASH
    // =====================
    [Header("Home / Leash")]
    [SerializeField] protected float maxDistanceFromHome = 10f;

    protected EnemyAudio enemyAudio;

    // =====================
    // COMPONENTS
    // =====================
    protected Rigidbody2D rb;
    protected Animator animator;
    protected VisionCone visionCone;
    protected EnemyAttackHitbox attackHitbox;
    protected Collider2D enemyCollider;
    protected Collider2D playerCollider;
    protected Transform playerTransform;

    // =====================
    // STATE
    // =====================
    protected Vector2 patrolCenter;
    protected Vector2 patrolTarget;
    protected float patrolSegmentTimer;
    protected float patrolStopTimer;

    protected float attackTimer;
    protected bool isChasing;
    protected bool isStunned;
    protected bool isSlowed;
    protected bool isTaunted;

    protected float chaseGraceTimer;
    protected float idleAfterLostTimer;
    protected Vector2 lastKnownPlayerPosition;
    protected Vector2 currentTauntTarget;

    protected Vector2 homePosition;
    protected bool returningHome;
    protected bool hasHomePosition;

    protected Coroutine stunRoutine;
    protected Coroutine slowRoutine;
    protected Coroutine tauntRoutine;
    protected bool hasPlayedAlertSFX;

    protected readonly int hashSpeed = Animator.StringToHash("Speed");
    protected readonly int hashAttack = Animator.StringToHash("Attack");

    protected enum State
    {
        Patrol,
        Chase,
        Attacking,
        Stunned,
        Taunted,
        LostChase,
        IdleAfterLost,
        ReturnHome
    }

    protected State state = State.Patrol;

    // =====================
    // UNITY
    // =====================
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        visionCone = GetComponentInChildren<VisionCone>();
        attackHitbox = GetComponentInChildren<EnemyAttackHitbox>();
        enemyCollider = GetComponent<Collider2D>();
        enemyAudio = GetComponent<EnemyAudio>();
    }

    protected virtual void Start()
    {
        patrolCenter = rb.position;
        PickNewPatrolTarget();

        if (visionCone != null)
        {
            // Đổi OnPlayerEnter thành OnPlayerSpotted để khớp với VisionCone mới
            visionCone.OnPlayerSpotted += OnPlayerSpotted;
            visionCone.OnPlayerLost += OnPlayerLost;
        }
    }

    protected virtual void Update()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (hasHomePosition && !returningHome &&
            Vector2.Distance(rb.position, homePosition) > maxDistanceFromHome)
        {
            returningHome = true;
            playerTransform = null;
            playerCollider = null;
            isChasing = false;
            state = State.ReturnHome;
        }

        switch (state)
        {
            case State.Stunned:
                rb.linearVelocity = Vector2.zero;
                break;

            case State.Taunted:
                UpdateTaunted();
                break;

            case State.Attacking:
                UpdateAttacking();
                break;

            case State.Chase:
                UpdateChase();
                break;

            case State.LostChase:
                UpdateLostChase();
                break;

            case State.IdleAfterLost:
                UpdateIdleAfterLost();
                break;

            case State.Patrol:
                UpdatePatrol();
                break;

            case State.ReturnHome:
                UpdateReturnHome();
                break;
        }

        animator.SetFloat(hashSpeed, rb.linearVelocity.magnitude);
    }

    // =====================
    // PATROL
    // =====================
    protected void UpdatePatrol()
    {
        if (patrolStopTimer > 0f)
        {
            patrolStopTimer -= Time.deltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        patrolSegmentTimer -= Time.deltaTime;
        Vector2 dir = patrolTarget - rb.position;

        if (dir.magnitude < 0.2f || patrolSegmentTimer <= 0f)
        {
            PickNewPatrolTarget();
            patrolStopTimer = patrolStopTime;
            return;
        }

        rb.linearVelocity = dir.normalized * GetCurrentSpeed(patrolSpeed);
        UpdateFacing(dir);
    }

    protected void PickNewPatrolTarget()
    {
        patrolTarget = patrolCenter + Random.insideUnitCircle * patrolRadius;
        patrolSegmentTimer = patrolSegmentTime;
    }

    // =====================
    // CHASE (DEFAULT = DEMO)
    // =====================
    protected virtual void UpdateChase()
    {
        if (playerTransform == null || playerCollider == null)
        {
            state = State.Patrol;
            return;
        }

        lastKnownPlayerPosition = playerTransform.position;

        ColliderDistance2D dist = enemyCollider.Distance(playerCollider);

        if (dist.isOverlapped || dist.distance <= attackDistance)
        {
            rb.linearVelocity = Vector2.zero;

            if (attackTimer <= 0f)
            {
                state = State.Attacking;
                attackTimer = attackCooldown;
                animator.SetTrigger(hashAttack);
            }
            return;
        }

        Vector2 dir = (Vector2)playerTransform.position - rb.position;
        rb.linearVelocity = dir.normalized * GetCurrentSpeed(chaseSpeed);
        UpdateFacing(dir);
    }

    // =====================
    // LOST CHASE
    // =====================
  
    protected virtual void UpdateLostChase()
    {
        if (playerTransform != null)
        {
            state = State.Chase;
            return;
        }

        if (chaseGraceTimer > 0f)
        {
            chaseGraceTimer -= Time.deltaTime;
            Vector2 dir = lastKnownPlayerPosition - rb.position;

            if (dir.magnitude < 0.2f)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            rb.linearVelocity = dir.normalized * GetCurrentSpeed(chaseSpeed);
            UpdateFacing(dir);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            idleAfterLostTimer = idleAfterLostDuration;
            state = State.IdleAfterLost;
        }
    }

    protected void UpdateIdleAfterLost()
    {
        rb.linearVelocity = Vector2.zero;

        if (idleAfterLostTimer > 0f)
        {
            idleAfterLostTimer -= Time.deltaTime;
            return;
        }

        hasPlayedAlertSFX = false; // ⭐ RESET TẠI ĐÂY

        PickNewPatrolTarget();
        state = State.Patrol;
    }


    protected void UpdateReturnHome()
    {
        Vector2 dir = homePosition - rb.position;

        if (dir.magnitude < 0.2f)
        {
            rb.linearVelocity = Vector2.zero;
            returningHome = false;

            hasPlayedAlertSFX = false; // ⭐ RESET

            PickNewPatrolTarget();
            state = State.Patrol;
            return;
        }

        rb.linearVelocity = dir.normalized * GetCurrentSpeed(chaseSpeed);
        UpdateFacing(dir);
    }


    // =====================
    // ATTACK
    // =====================
    protected virtual void UpdateAttacking()
    {
        rb.linearVelocity = Vector2.zero;

        if (attackTimer <= 0f)
        {
            state = isChasing ? State.Chase : State.LostChase;
        }
    }

    // =====================
    // TAUNT
    // =====================
    protected virtual void UpdateTaunted()
    {
        Vector2 dir = currentTauntTarget - rb.position;
        float distance = dir.magnitude;
        if (distance > 0.5f)
        {
            rb.linearVelocity = dir.normalized * GetCurrentSpeed(chaseSpeed);
            UpdateFacing(dir);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

    }

    // =====================
    // VISION
    // =====================
    protected void OnPlayerSpotted(Transform player)
    {
        playerTransform = player;
        playerCollider = player.GetComponent<Collider2D>();
        isChasing = true;

        chaseGraceTimer = 0f;
        idleAfterLostTimer = 0f;

        if (!hasPlayedAlertSFX)
        {
            enemyAudio?.PlayAlertSFX();   // 🔊 CHỈ 1 LẦN
            hasPlayedAlertSFX = true;
        }

        if (!isStunned && !isTaunted)
            state = State.Chase;
    }




    protected void OnPlayerLost()
    {
        if (playerTransform != null)
            lastKnownPlayerPosition = playerTransform.position;

        playerTransform = null;
        playerCollider = null;
        isChasing = false;

        hasPlayedAlertSFX = false; // 🔁 cho lần phát hiện sau
        enemyAudio.StopAllEffects();

        chaseGraceTimer = chaseGraceDuration;
        state = State.LostChase;
    }


    // =====================
    // EFFECT API
    // =====================
    public void StunDuration(float duration)
    {
        if (stunRoutine != null) StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        state = State.Stunned;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        state = isChasing ? State.Chase : State.Patrol;
    }

    public void SlowDuration(float duration, float multiplier)
    {
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(SlowRoutine(duration, multiplier));
    }

    IEnumerator SlowRoutine(float duration, float multiplier)
    {
        isSlowed = true;
        float old = slowMultiplier;
        slowMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        slowMultiplier = old;
        isSlowed = false;
    }

    public void TauntDuration(float duration, Vector2 source)
    {
        if (tauntRoutine != null) StopCoroutine(tauntRoutine);
        tauntRoutine = StartCoroutine(TauntRoutine(duration));
    }
    public void ApplyTaunt(float duration, Vector3 position)
    {
        if (tauntRoutine != null)
        {
            StopCoroutine(tauntRoutine);
        }
        currentTauntTarget = position;
        TauntDuration(duration, position);
    }
    IEnumerator TauntRoutine(float duration)
    {
        isTaunted = true;
        state = State.Taunted;
        yield return new WaitForSeconds(duration);
        isTaunted = false;
        state = (playerTransform != null) ? State.Chase : State.Patrol;
    }

    // =====================
    // HELPERS
    // =====================
    protected float GetCurrentSpeed(float baseSpeed)
    {
        return isSlowed ? baseSpeed * slowMultiplier : baseSpeed;
    }

    protected void UpdateFacing(Vector2 dir)
    {
        if (dir.x > 0.01f)
            transform.localScale = Vector3.one;
        else if (dir.x < -0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    // =====================
    // ANIMATION EVENT
    // =====================
    public void EnableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.EnableHitbox();
    }

    public void DisableAttackHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.DisableHitbox();
    }

    // =====================
    // HOME
    // =====================
    public void SetHomePosition(Vector2 pos)
    {
        homePosition = pos;
        patrolCenter = pos;
        hasHomePosition = true;
    }
}