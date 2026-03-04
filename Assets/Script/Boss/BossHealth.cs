using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("UI")]
    [SerializeField] private Slider bossHpSlider;
    [SerializeField] private Image bossIconImage;        // 🔥 ICON UI
    [SerializeField] private Sprite[] bossIconForms;     // 🔥 ICON THEO PHASE

    [Header("Phase Settings")]
    [SerializeField] private SpriteRenderer bossSprite;
    [SerializeField] private Sprite[] bossForms;
    [SerializeField] private BossAttackController attackController;

    [SerializeField] private GameObject door;

    private int currentPhase = 1;
    private bool isDead = false;

    private void Update()
    {
        hack();
    }
    void Start()
    {
        currentHealth = maxHealth;
        bossHpSlider.maxValue = maxHealth;
        bossHpSlider.value = currentHealth;

        // Set icon phase 1 ban đầu
        UpdateIcon(1);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        bossHpSlider.value = currentHealth;

        CheckPhase();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void CheckPhase()
    {
        float percent = currentHealth / maxHealth;

        if (percent <= 0.25f && currentPhase < 4)
            EnterPhase(4);
        else if (percent <= 0.50f && currentPhase < 3)
            EnterPhase(3);
        else if (percent <= 0.75f && currentPhase < 2)
            EnterPhase(2);
    }

    void EnterPhase(int phase)
    {
        currentPhase = phase;
        Debug.Log("Boss vào Phase " + phase);

        // 🔥 Đổi sprite boss
        if (bossForms.Length >= phase)
            bossSprite.sprite = bossForms[phase - 1];

        // 🔥 Đổi icon UI
        UpdateIcon(phase);

        switch (phase)
        {
            case 2:
                attackController.SetAttackInterval(6f);
                break;
            case 3:
                attackController.SetAttackInterval(4f);
                break;
            case 4:
                attackController.SetAttackInterval(2f);
                break;
        }
    }

    void UpdateIcon(int phase)
    {
        if (bossIconImage != null && bossIconForms.Length >= phase)
        {
            bossIconImage.sprite = bossIconForms[phase - 1];
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Boss defeated!");

        if (attackController != null)
            attackController.HandleBossDeath(this);
        else
            FinalDeath();
    }

    public void FinalDeath()
    {
        if (door != null)
            Destroy(door);

        Destroy(gameObject);
    }

    public void DamageOnePhase()
    {
        float phaseDamage = maxHealth * 0.25f;
        TakeDamage(phaseDamage);
    }

    public void hack()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            DamageOnePhase();
                }
    }
}