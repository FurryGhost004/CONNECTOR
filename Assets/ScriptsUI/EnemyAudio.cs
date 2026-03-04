using UnityEngine;
using UnityEngine.Audio;

public class EnemyAudio : MonoBehaviour
{
    [Header("Enemy SFX")]
    public AudioClip attackSFX;
    public AudioClip hitSFX;
    public AudioClip alertSFX;

    [Header("Boss SFX")]
    public AudioClip laserChargeClip;
    public AudioClip laserFireClip;
    public AudioClip lightningWarningClip;
    public AudioClip lightningStrikeClip;

    [Header("Audio Mixer")]
    public AudioMixerGroup sfxMixerGroup;

    private AudioSource attackSource;   // loop / continuous
    private AudioSource oneShotSource;  // UI / hit / quick sound
    private AudioSource spatialSource;  // 3D sound

    void Awake()
    {
        // Attack source
        attackSource = CreateSource(false, 0f);

        // One-shot 2D
        oneShotSource = CreateSource(false, 0f);

        // 3D source
        spatialSource = CreateSource(false, 1f);
    }

    AudioSource CreateSource(bool loop, float spatialBlend)
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = loop;
        src.spatialBlend = spatialBlend;
        src.outputAudioMixerGroup = sfxMixerGroup;
        return src;
    }

    // =========================
    // BASIC ENEMY SOUND
    // =========================

    public void PlayAttackEffect()
    {
        if (attackSFX == null || attackSource.isPlaying) return;

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

    // =========================
    // BOSS LASER
    // =========================

    public void PlayLaserCharge()
    {
        if (laserChargeClip == null) return;
        oneShotSource.PlayOneShot(laserChargeClip);
    }

    public void PlayLaserFire()
    {
        if (laserFireClip == null) return;
        oneShotSource.PlayOneShot(laserFireClip);
    }

    // =========================
    // BOSS LIGHTNING (3D)
    // =========================

    public void PlayLightningWarning(Vector3 pos)
    {
        oneShotSource.PlayOneShot(lightningWarningClip);
    }

    public void PlayLightningStrike(Vector3 pos)
    {
        oneShotSource.PlayOneShot(lightningStrikeClip);
    }

    void Play3DSound(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;

        spatialSource.transform.position = position;
        spatialSource.clip = clip;
        spatialSource.Play();
    }

    // =========================
    // CLEANUP
    // =========================

    public void StopAllEffects()
    {
        attackSource.Stop();
        oneShotSource.Stop();
        spatialSource.Stop();
    }
}