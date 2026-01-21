using UnityEngine;
using UnityEngine.Audio;

public class EnemyAudio : MonoBehaviour
{
    [Header("Enemy SFX")]
    public AudioClip attackSFX;
    public AudioClip hitSFX;
    public AudioClip alertSFX;

    [Header("Audio Mixer")]
    public AudioMixerGroup sfxMixerGroup; // 🔥 KÉO SFX GROUP VÀO ĐÂY

    private AudioSource attackSource;
    private AudioSource oneShotSource;

    void Awake()
    {
        // Attack source
        attackSource = gameObject.AddComponent<AudioSource>();
        attackSource.playOnAwake = false;
        attackSource.loop = false;
        attackSource.outputAudioMixerGroup = sfxMixerGroup; // ⭐ QUAN TRỌNG

        // One-shot source
        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.playOnAwake = false;
        oneShotSource.loop = false;
        oneShotSource.outputAudioMixerGroup = sfxMixerGroup; // ⭐ QUAN TRỌNG
    }

    public void PlayAttackEffect()
    {
        if (attackSFX == null) return;
        if (attackSource.isPlaying) return;

        attackSource.clip = attackSFX;
        attackSource.Play();
    }

    public void StopAttackEffect()
    {
        if (attackSource.isPlaying)
            attackSource.Stop();
    }

    public void PlayHitSFX()
    {
        if (hitSFX == null) return;
        oneShotSource.PlayOneShot(hitSFX);
    }

    public void PlayAlertSFX()
    {
        if (alertSFX == null) return;
        oneShotSource.PlayOneShot(alertSFX);
    }

    public void StopAllEffects()
    {
        attackSource.Stop();
        oneShotSource.Stop();
    }
}
