using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Enemy SFX")]
    public AudioClip attackSFX;
    public AudioClip hitSFX;
    public AudioClip alertSFX;

    private AudioSource attackSource; // dùng cho gồng + dash
    private AudioSource oneShotSource; // dùng cho alert / hit

    void Awake()
    {
        // Source cho attack (có thể stop)
        attackSource = gameObject.AddComponent<AudioSource>();
        attackSource.playOnAwake = false;
        attackSource.loop = false;

        // Source cho one-shot
        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.playOnAwake = false;
        oneShotSource.loop = false;
    }

    // 🔊 Phát sound gồng / attack (1 lần)
    public void PlayAttackEffect()
    {
        if (attackSFX == null) return;
        if (attackSource.isPlaying) return;

        attackSource.clip = attackSFX;
        attackSource.Play();
    }

    // 🔇 Dừng sound gồng khi dash xong / bị stun
    public void StopAttackEffect()
    {
        if (attackSource.isPlaying)
            attackSource.Stop();
    }

    // 💥 Khi enemy bị đánh
    public void PlayHitSFX()
    {
        if (hitSFX == null) return;
        oneShotSource.PlayOneShot(hitSFX);
    }
    public void StopAllEffects()
    {
        if (attackSource != null && attackSource.isPlaying)
            attackSource.Stop();

        if (oneShotSource != null && oneShotSource.isPlaying)
            oneShotSource.Stop();
    }


    // 🚨 Khi phát hiện player (CHỈ 1 LẦN)
    public void PlayAlertSFX()
    {
        if (alertSFX == null) return;
        oneShotSource.PlayOneShot(alertSFX);
    }
}
