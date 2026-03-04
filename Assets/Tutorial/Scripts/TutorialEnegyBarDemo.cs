using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Energy bar demo đơn giản - chỉ đổi màu player khi stealth
/// Version 3 - Simple color change only
/// </summary>
public class TutorialEnergyBarDemo_Simple : MonoBehaviour
{
    [Header("Energy Bar Components")]
    [SerializeField] private Slider energySlider; // Slider component
    [SerializeField] private Image sliderFillImage; // Fill image để đổi màu

    [Header("Player Image Control")]
    [SerializeField] private Image playerImage; // CHỈ 1 PLAYER IMAGE DUY NHẤT

    [Header("Player Colors")]
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 1f); // Trắng - bình thường
    [SerializeField] private Color stealthColor = new Color(0.31f, 0.31f, 0.39f, 1f); // Xám - tàng hình (80, 80, 100)
    [SerializeField] private float colorTransitionSpeed = 2f; // Tốc độ đổi màu

    [Header("Animation Timing")]
    [SerializeField] private float startDelay = 1f; // Đợi 1 giây trước khi bắt đầu
    [SerializeField] private float depletionSpeed = 0.125f; // Giảm CHẬM (12.5% mỗi giây ~ 4.8s để giảm từ 100%→40%)
    [SerializeField] private float rechargeSpeed = 0.15f; // Hồi CHẬM (15% mỗi giây ~ 4s để hồi từ 40%→100%)

    [Header("Stealth Thresholds")]
    [SerializeField] private float stealthStartEnergy = 0.9f; // Bắt đầu stealth ở 90%
    [SerializeField] private float stealthEndEnergy = 0.4f; // Kết thúc stealth ở 40%

    [Header("Energy Bar Colors")]
    [SerializeField] private Color fullEnergyColor = new Color(0f, 1f, 1f, 1f); // Cyan
    [SerializeField] private Color midEnergyColor = new Color(1f, 0.67f, 0f, 1f); // Orange
    [SerializeField] private Color lowEnergyColor = new Color(1f, 0f, 0f, 1f); // Red

    [Header("Pulse Effect")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 3f;

    // Private variables
    private float currentEnergy = 1f;
    private bool isDepleting = false; // Ban đầu KHÔNG giảm (đợi 1s)
    private bool isStealthActive = false;
    private float startTimer = 0f;
    private bool hasStarted = false;
    private Color targetPlayerColor;
    private float pulseTimer = 0f;

    private void Start()
    {
        if (energySlider == null)
        {
            Debug.LogWarning("⚠️ Energy Slider not assigned!");
            return;
        }

        if (playerImage == null)
        {
            Debug.LogWarning("⚠️ Player Image not assigned!");
            return;
        }

        // Get fill image from slider
        if (energySlider.fillRect != null)
        {
            sliderFillImage = energySlider.fillRect.GetComponent<Image>();
        }

        // Khởi tạo
        currentEnergy = 1f;
        isDepleting = false;
        isStealthActive = false;
        hasStarted = false;
        startTimer = 0f;

        // Set màu ban đầu
        playerImage.color = normalColor;
        targetPlayerColor = normalColor;

        // Set slider value
        energySlider.value = currentEnergy;

        UpdateEnergyBar();

        Debug.Log("✅ Energy Bar Demo initialized - Waiting " + startDelay + "s before start");
    }

    private void Update()
    {
        // Chờ 1 giây trước khi bắt đầu
        if (!hasStarted)
        {
            startTimer += Time.unscaledDeltaTime;
            if (startTimer >= startDelay)
            {
                hasStarted = true;
                isDepleting = true;
                Debug.Log("▶️ Starting energy depletion...");
            }
            return; // Không làm gì cả trong lúc chờ
        }

        // === ANIMATION CYCLE ===

        if (isDepleting)
        {
            // GIAI ĐOẠN 1: GIẢM từ 100% → 40%
            currentEnergy -= depletionSpeed * Time.unscaledDeltaTime;

            // Kích hoạt stealth khi xuống dưới 90%
            if (currentEnergy <= stealthStartEnergy && !isStealthActive)
            {
                ActivateStealth();
            }

            // Khi xuống 40%: Chuyển sang hồi năng lượng
            if (currentEnergy <= stealthEndEnergy)
            {
                currentEnergy = stealthEndEnergy; // Clamp ở 40%
                isDepleting = false;
                DeactivateStealth();
                Debug.Log("🔄 Switching to recharge mode at 40%");
            }
        }
        else
        {
            // GIAI ĐOẠN 2: HỒI từ 40% → 100%
            currentEnergy += rechargeSpeed * Time.unscaledDeltaTime;

            // Khi đầy năng lượng: Lặp lại cycle
            if (currentEnergy >= 1f)
            {
                currentEnergy = 1f;
                isDepleting = true;
                Debug.Log("🔁 Energy full! Restarting cycle...");
            }
        }

        // Clamp energy
        currentEnergy = Mathf.Clamp01(currentEnergy);

        // Cập nhật visuals
        UpdateEnergyBar();
        UpdatePlayerColor();

        // Pulse effect khi năng lượng thấp
        if (enablePulse && currentEnergy < 0.3f)
        {
            ApplyPulseEffect();
        }
    }

    /// <summary>
    /// Kích hoạt chế độ stealth - Đổi màu player sang xám
    /// </summary>
    private void ActivateStealth()
    {
        isStealthActive = true;
        targetPlayerColor = stealthColor;
        Debug.Log("👻 Stealth ACTIVE - Player color changing to gray");
    }

    /// <summary>
    /// Tắt chế độ stealth - Đổi màu player về bình thường
    /// </summary>
    private void DeactivateStealth()
    {
        isStealthActive = false;
        targetPlayerColor = normalColor;
        Debug.Log("👤 Stealth INACTIVE - Player color returning to normal");
    }

    /// <summary>
    /// Cập nhật màu của player (smooth transition)
    /// </summary>
    private void UpdatePlayerColor()
    {
        if (playerImage == null) return;

        // Smooth transition sang màu mục tiêu
        playerImage.color = Color.Lerp(
            playerImage.color,
            targetPlayerColor,
            Time.unscaledDeltaTime * colorTransitionSpeed
        );
    }

    /// <summary>
    /// Cập nhật energy bar
    /// </summary>
    private void UpdateEnergyBar()
    {
        if (energySlider == null) return;

        // Update slider value (giảm từ 1 → 0)
        energySlider.value = currentEnergy;

        // Update color của Fill image
        if (sliderFillImage != null)
        {
            Color barColor;

            if (currentEnergy <= 0.25f)
            {
                barColor = lowEnergyColor; // Red
            }
            else if (currentEnergy <= 0.5f)
            {
                // Red → Orange
                float t = (currentEnergy - 0.25f) / 0.25f;
                barColor = Color.Lerp(lowEnergyColor, midEnergyColor, t);
            }
            else
            {
                // Orange → Cyan
                float t = (currentEnergy - 0.5f) / 0.5f;
                barColor = Color.Lerp(midEnergyColor, fullEnergyColor, t);
            }

            sliderFillImage.color = barColor;
        }
    }

    /// <summary>
    /// Hiệu ứng pulse khi năng lượng thấp
    /// </summary>
    private void ApplyPulseEffect()
    {
        if (sliderFillImage == null) return;

        pulseTimer += Time.unscaledDeltaTime * pulseSpeed;

        float alpha = Mathf.Lerp(0.6f, 1f, (Mathf.Sin(pulseTimer) + 1f) / 2f);

        Color currentColor = sliderFillImage.color;
        currentColor.a = alpha;
        sliderFillImage.color = currentColor;
    }

    /// <summary>
    /// Reset về trạng thái ban đầu
    /// </summary>
    public void ResetAnimation()
    {
        currentEnergy = 1f;
        isDepleting = false;
        isStealthActive = false;
        hasStarted = false;
        startTimer = 0f;
        pulseTimer = 0f;

        if (playerImage != null)
            playerImage.color = normalColor;

        targetPlayerColor = normalColor;

        UpdateEnergyBar();

        Debug.Log("🔄 Animation reset");
    }

    /// <summary>
    /// Pause/Resume
    /// </summary>
    public void SetPaused(bool paused)
    {
        enabled = !paused;
    }
}