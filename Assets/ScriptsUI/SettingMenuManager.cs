using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingManager : MonoBehaviour
{
    public Slider masterVol;
    public Slider musicVol;
    public Slider sfxVol;

    public AudioMixer mainAudioMixer;
    public GameObject settingsPanel;

    private const string MASTER_KEY = "MasterVolValue";
    private const string MUSIC_KEY = "MusicVolValue";
    private const string SFX_KEY = "SfxVolValue";

    void Start()
    {
        LoadSettings();
        
        masterVol.onValueChanged.AddListener(SetMasterVolume);
        musicVol.onValueChanged.AddListener(SetMusicVolume);
        sfxVol.onValueChanged.AddListener(SetSfxVolume);
    }
    
    void SetMasterVolume(float value)
    {
        mainAudioMixer.SetFloat("MasterVol", value);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    void SetMusicVolume(float value)
    {
        mainAudioMixer.SetFloat("MusicVol", value);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        Debug.Log("Music Volume: " + value);
        mainAudioMixer.SetFloat("MusicVol", value);
    }

    void SetSfxVolume(float value)
    {
        mainAudioMixer.SetFloat("SfxVol", value);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    public void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 0f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 0f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 0f);

        masterVol.value = master;
        musicVol.value = music;
        sfxVol.value = sfx;

        mainAudioMixer.SetFloat("MasterVol", master);
        mainAudioMixer.SetFloat("MusicVol", music);
        mainAudioMixer.SetFloat("SfxVol", sfx);

        Debug.Log("🔊 Settings Loaded");
    }

    public void OpenPanel()
    {
        settingsPanel.SetActive(true);
        LoadSettings();
    }

    public void ClosePanel()
    {
        settingsPanel.SetActive(false);
        PlayerPrefs.Save();
    }
}