using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider masterVol;
    public Slider musicVol;
    public Slider sfxVol;

    [Header("Audio Mixer")]
    public AudioMixer mainAudioMixer;

    [Header("UI Panel")]
    public GameObject settingsPanel;

    void Start()
    {
        LoadSettings();

        if (masterVol != null)
            masterVol.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVol != null)
            musicVol.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVol != null)
            sfxVol.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    // =========================
    // SLIDER EVENTS (AUTO SAVE)
    // =========================
    void OnMasterVolumeChanged(float value)
    {
        mainAudioMixer.SetFloat("MasterVol", value);
        SaveVolume("MasterVolValue", value);
    }

    void OnMusicVolumeChanged(float value)
    {
        mainAudioMixer.SetFloat("MusicVol", value);
        SaveVolume("MusicVolValue", value);
    }

    void OnSfxVolumeChanged(float value)
    {
        mainAudioMixer.SetFloat("SfxVol", value);
        SaveVolume("SfxVolValue", value);
    }

    // =========================
    // SAVE / LOAD
    // =========================
    void SaveVolume(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    // 🔥 QUAN TRỌNG: PHẢI LÀ PUBLIC
    public void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolValue", 0f);
        float music = PlayerPrefs.GetFloat("MusicVolValue", 0f);
        float sfx = PlayerPrefs.GetFloat("SfxVolValue", 0f);

        if (masterVol != null) masterVol.value = master;
        if (musicVol != null) musicVol.value = music;
        if (sfxVol != null) sfxVol.value = sfx;

        mainAudioMixer.SetFloat("MasterVol", master);
        mainAudioMixer.SetFloat("MusicVol", music);
        mainAudioMixer.SetFloat("SfxVol", sfx);

        Debug.Log("🔊 Settings Loaded");
    }

    // =========================
    // PANEL CONTROL
    // =========================
    public void OpenPanel()
    {
        settingsPanel.SetActive(true);
        LoadSettings();
    }

    public void ClosePanel()
    {
        settingsPanel.SetActive(false);
    }
}
