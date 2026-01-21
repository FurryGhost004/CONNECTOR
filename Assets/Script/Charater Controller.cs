using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharaterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    private float originalSpeed;

    [Header("References")]
    [SerializeField] private GameObject character;
    [SerializeField] GameObject[] starImage;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] TextMeshProUGUI rewardText;

    [Header("Skill Settings")]
    [SerializeField] private float staminaCostPerSecond = 20f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PickupItem pickupItem;
    Timer timer;

    // Biến trạng thái
    public bool isHidden { get; private set; } = false;
    private bool isKnockback;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = FindAnyObjectByType<Timer>();

        originalSpeed = moveSpeed;   // lưu tốc độ ban đầu
    }

    void Update()
    {
        if (isKnockback) return;

        Move();
        CheckItem();
        Skill();
    }


    // =========================
    // Movement & Animation
    // =========================
    void Move()
    {
        if (isKnockback) return;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, verticalInput * moveSpeed);

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1f, 1f, 1f);

        // Animation bằng GetKey
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    // =========================
    // Pickup Item
    // =========================
    void CheckItem()
    {
        if (pickupItem != null && Input.GetKeyDown(KeyCode.F))
        {
            if (InventoryManager.Instance.AddItem(pickupItem.itemData))
            {
                Destroy(pickupItem.gameObject);
                pickupItem = null;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("item"))
        {
            pickupItem = collision.GetComponent<PickupItem>();
        }
        if (collision.gameObject.CompareTag("Goal"))
        {
            Victory();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("item"))
        {
            if (pickupItem != null && pickupItem.gameObject == collision.gameObject)
            {
                pickupItem = null;
            }
        }
    }

    // =========================
    // Skill: Nhấn C để ẩn
    // =========================
    void Skill()
    {
        if (Input.GetKey(KeyCode.C) && HealthSystem.Instance.currentStamina > 0)
        {
            DarkenCharacter();
            isHidden = true;
            moveSpeed = originalSpeed * 0.5f;
            HealthSystem.Instance.ConsumeStamina(staminaCostPerSecond);
        }
        else
        {
            // ✅ CHỈ reset màu khi KHÔNG vô địch
            if (!HealthSystem.Instance.IsInvincible())
            {
                ResetCharacterColor();
            }

            isHidden = false;
            moveSpeed = originalSpeed;
        }
    }


    void DarkenCharacter()
    {
        spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f); // tối hơn
    }

    void ResetCharacterColor()
    {
        spriteRenderer.color = Color.white;
    }

    // =========================
    // Victory
    // =========================
    void Victory()
    {
        float timeleft = timer.GetRemainTime();
        int star = 1;
        timer.StopTimer();

        if (timeleft > 240)
        {
            star = 5;
        }
        else if (timeleft > 180)
        {
            star = 4;
        }
        else if (timeleft > 120)
        {
            star = 3;
        }
        else if (timeleft > 60)
        {
            star = 2;
        }
        else if (timeleft > 0)
        {
            star = 1;
        }

        // Hiện sao
        for (int i = 0; i < star; i++)
        {
            if (i < starImage.Length)
            {
                starImage[i].SetActive(true);
            }
        }

        // Thưởng tiền
        int reward = star * 50;
        int currentMoney = PlayerPrefs.GetInt("Money", 0);
        PlayerPrefs.SetInt("Money", currentMoney + reward);
        PlayerPrefs.Save();

        rewardText.text = "Your Payment: + " + reward.ToString();
        victoryPanel.SetActive(true);

        moveSpeed = 0;

        // Lưu sao cao nhất
        string key = SceneManager.GetActiveScene().name + "_Star";
        int bestStar = PlayerPrefs.GetInt(key, 0);
        if (star > bestStar)
        {
            PlayerPrefs.SetInt(key, star);
            PlayerPrefs.Save();
        }
    }
    public void Knockback(Vector2 direction, float force, float duration)
    {
        if (isKnockback) return;
        StartCoroutine(KnockbackRoutine(direction, force, duration));
    }
    IEnumerator KnockbackRoutine(Vector2 dir, float force, float duration)
    {
        isKnockback = true;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir.normalized * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }



}
