using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;
    public float healthBarSpeed = 2f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public Slider staminaSlider;
    public float staminaRegenRate = 15f; // Tốc độ hồi thể lực mỗi giây
    public float staminaBarSpeed = 5f;

    [Header("Damage Settings")]
    public float damageFlashDuration = 0.2f;
    public Color damageFlashColor = Color.red;

    [Header("Invincibility Settings")]
    public float invincibleDuration = 1f;   // Thời gian vô địch
    public float flashInterval = 0.1f;       // Tốc độ nhấp nháy

    private bool isInvincible = false;

    [Header("Knockback Settings")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.15f;


    [Header("References")]
    public FailedManager failedManager;

    private SpriteRenderer playerSprite;
    private Color originalColor;
    private float targetHealthValue;
    private float targetStaminaValue;
    private Image healthFill;
    private Image staminaFill;
    

    public static HealthSystem Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        playerSprite = GetComponentInChildren<SpriteRenderer>();
        if (playerSprite != null)
            originalColor = playerSprite.color;
    }


    void Start()
    {
        // Khởi tạo thông số Máu
        currentHealth = maxHealth;
        targetHealthValue = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            healthFill = healthSlider.fillRect.GetComponent<Image>();
        }

        // Khởi tạo thông số Thể lực
        currentStamina = maxStamina;
        targetStaminaValue = maxStamina;
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = maxStamina;
            staminaFill = staminaSlider.fillRect.GetComponent<Image>();
            // Mặc định thanh thể lực màu xanh dương
            if (staminaFill != null) staminaFill.color = new Color(0.2f, 0.5f, 1f);
        }
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        HandleHealthUI();
        HandleStaminaLogic();

        // Test phím tắt nhanh (Optional)
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(20, transform.position + Vector3.left);
        }

    }

    // =========================
    // LOGIC MÁU (HEALTH)
    // =========================
    void HandleHealthUI()
    {
        if (healthSlider != null)
        {
            // Hiệu ứng thanh máu rút từ từ
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealthValue, Time.deltaTime * healthBarSpeed);
            UpdateHealthBarColor();
        }
    }

    public void TakeDamage(float damage, Vector2 attackSource)
    {
        if (currentHealth <= 0 || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        targetHealthValue = currentHealth;

        // 🔥 Knockback
        CharaterController player = GetComponent<CharaterController>();
        if (player != null)
        {
            Vector2 dir = (Vector2)transform.position - attackSource;
            player.Knockback(dir.normalized, knockbackForce, knockbackDuration);
        }

        StopAllCoroutines();
        StartCoroutine(InvincibilityFlash());

        if (currentHealth <= 0)
            Die();
    }




    void UpdateHealthBarColor()
    {
        if (healthFill == null) return;

        float healthPercent = currentHealth / maxHealth;
        if (healthPercent > 0.6f) healthFill.color = Color.green;
        else if (healthPercent > 0.3f) healthFill.color = Color.yellow;
        else healthFill.color = Color.red;
    }

    // =========================
    // LOGIC THỂ LỰC (STAMINA)
    // =========================
    void HandleStaminaLogic()
    {
        // CHỈ HỒI THỂ LỰC KHI: Không nhấn phím C (dùng skill) và chưa đầy thể lực
        if (!Input.GetKey(KeyCode.C) && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            targetStaminaValue = currentStamina;
        }

        // Cập nhật Slider UI mượt mà
        if (staminaSlider != null)
        {
            staminaSlider.value = Mathf.Lerp(staminaSlider.value, targetStaminaValue, Time.deltaTime * staminaBarSpeed);
        }
    }

    // Hàm gọi từ CharacterController để trừ thể lực từ từ mỗi khung hình
    public void ConsumeStamina(float amountPerSecond)
    {
        currentStamina -= amountPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        targetStaminaValue = currentStamina;
    }

    // Hàm dùng thể lực tức thì (ví dụ: cho hành động Nhảy)
    public bool UseStaminaInstant(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            targetStaminaValue = currentStamina;
            return true;
        }
        return false;
    }

    // =========================
    // HIỆU ỨNG & TRẠNG THÁI
    // =========================
    IEnumerator DamageFlash()
    {
        if (playerSprite != null)
        {
            playerSprite.color = damageFlashColor;
            yield return new WaitForSeconds(damageFlashDuration);
            playerSprite.color = originalColor;
        }
    }

    IEnumerator InvincibilityFlash()
    {
        isInvincible = true;

        float timer = 0f;
        bool invisible = false;

        while (timer < invincibleDuration)
        {
            if (playerSprite != null)
            {
                Color c = playerSprite.color;
                c.a = invisible ? 0.2f : 1f;   // mờ / rõ
                playerSprite.color = c;
                invisible = !invisible;
            }

            timer += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        // reset
        Color reset = playerSprite.color;
        reset.a = 1f;
        playerSprite.color = reset;

        isInvincible = false;
    }

    void ApplyKnockback(Vector2 sourcePos)
    {
        CharaterController controller = GetComponent<CharaterController>();
        if (controller != null)
        {
            Vector2 dir = ((Vector2)transform.position - sourcePos).normalized;
            controller.Knockback(dir, knockbackForce, knockbackDuration);
        }
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }


    void Die()
    {
        Debug.Log("Player đã chết!");
        if (failedManager != null) failedManager.ShowFailed();

        // Tắt script điều khiển khi chết
        CharaterController controller = GetComponent<CharaterController>();
        if (controller != null) controller.enabled = false;
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        targetHealthValue = currentHealth;
    }
    public void RestoreEnergy(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        targetStaminaValue = currentStamina;
    }

    public void StartRecoverHealth(float healAmount, float duration)
    {
        StartCoroutine(RecoverHealthOverTime(healAmount, duration));
    }
    IEnumerator RecoverHealthOverTime(float healAmount, float duration)
    {
        float amountPerSecond = healAmount / duration;
        float timer = 0f;
        while (timer < duration)
        {
            if (currentHealth < maxHealth)
            {
                Heal(amountPerSecond * Time.deltaTime);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log($"Heal {amountPerSecond} HP.");
    }

}