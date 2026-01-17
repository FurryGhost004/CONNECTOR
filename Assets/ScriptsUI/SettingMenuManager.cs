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

    // Lưu giá trị tạm khi kéo slider (chưa Apply)
    private float tempMasterVol;
    private float tempMusicVol;
    private float tempSfxVol;

    // Lưu giá trị đã lưu gần nhất (để reset khi Cancel)
    private float savedMasterVol;
    private float savedMusicVol;
    private float savedSfxVol;

    // Flag để kiểm tra có thay đổi chưa lưu không
    private bool hasUnsavedChanges = false;

    void Start()
    {
        LoadSettings();

        // Lắng nghe sự kiện thay đổi slider
        if (masterVol != null)
            masterVol.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (musicVol != null)
            musicVol.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxVol != null)
            sfxVol.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    // === KHI KÉO SLIDER (REAL-TIME PREVIEW) ===
    private void OnMasterVolumeChanged(float value)
    {
        tempMasterVol = value;
        mainAudioMixer.SetFloat("MasterVol", value);

        // Đồng bộ với AudioManager
        if (AudioManager.Instance != null)
            AudioManager.Instance.UpdateVolume("MasterVol", value);

        hasUnsavedChanges = true; // Đánh dấu có thay đổi
    }

    private void OnMusicVolumeChanged(float value)
    {
        tempMusicVol = value;
        mainAudioMixer.SetFloat("MusicVol", value);

        if (AudioManager.Instance != null)
            AudioManager.Instance.UpdateVolume("MusicVol", value);

        hasUnsavedChanges = true;
    }

    private void OnSfxVolumeChanged(float value)
    {
        tempSfxVol = value;
        mainAudioMixer.SetFloat("SfxVol", value);

        if (AudioManager.Instance != null)
            AudioManager.Instance.UpdateVolume("SfxVol", value);

        hasUnsavedChanges = true;
    }

    // === NÚT APPLY: LƯU VÀO PLAYERPREFS ===
    public void Apply()
    {
        // Lưu giá trị hiện tại vào PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolValue", tempMasterVol);
        PlayerPrefs.SetFloat("MusicVolValue", tempMusicVol);
        PlayerPrefs.SetFloat("SfxVolValue", tempSfxVol);
        PlayerPrefs.Save();

        // Cập nhật giá trị đã lưu
        savedMasterVol = tempMasterVol;
        savedMusicVol = tempMusicVol;
        savedSfxVol = tempSfxVol;

        hasUnsavedChanges = false; // Đã lưu rồi

        Debug.Log("✅ Settings đã được lưu!");
    }

    // === NÚT CLOSE: ĐÓNG PANEL ===
    public void ClosePanel()
    {
        // ⭐ PHIÊN BẢN 1: KHÔNG HỎI, ĐÓNG THẲNG
        ResetToSavedValues();
        settingsPanel.SetActive(false);

        Debug.Log("❌ Đóng panel, reset về giá trị đã lưu");
    }

    // === NÚT CLOSE: ĐÓNG PANEL (CÓ HỎI XÁC NHẬN) ===
    // ⭐ BẠN DÙNG HÀM NÀY NẾU MUỐN HỎI XÁC NHẬN
    /*
    public void ClosePanel()
    {
        if (hasUnsavedChanges)
        {
            // Hiển thị popup xác nhận
            // Bạn cần tạo 1 UI Panel confirm riêng
            ShowConfirmDialog();
        }
        else
        {
            // Không có thay đổi, đóng luôn
            settingsPanel.SetActive(false);
        }
    }

    private void ShowConfirmDialog()
    {
        // Code hiển thị popup "Bạn chưa lưu, có muốn thoát không?"
        // Nếu YES → CloseWithoutSaving()
        // Nếu NO → Quay lại panel
        Debug.Log("⚠️ Bạn có thay đổi chưa lưu!");
    }

    public void CloseWithoutSaving()
    {
        ResetToSavedValues();
        settingsPanel.SetActive(false);
        Debug.Log("❌ Đóng mà không lưu, reset về giá trị cũ");
    }
    */

    // === RESET VỀ GIÁ TRỊ ĐÃ LƯU ===
    private void ResetToSavedValues()
    {
        // Đặt lại slider về giá trị đã lưu
        masterVol.value = savedMasterVol;
        musicVol.value = savedMusicVol;
        sfxVol.value = savedSfxVol;

        // Apply vào AudioMixer
        mainAudioMixer.SetFloat("MasterVol", savedMasterVol);
        mainAudioMixer.SetFloat("MusicVol", savedMusicVol);
        mainAudioMixer.SetFloat("SfxVol", savedSfxVol);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateVolume("MasterVol", savedMasterVol);
            AudioManager.Instance.UpdateVolume("MusicVol", savedMusicVol);
            AudioManager.Instance.UpdateVolume("SfxVol", savedSfxVol);
        }

        hasUnsavedChanges = false;
    }

    // === LOAD SETTINGS TỪ PLAYERPREFS ===
    public void LoadSettings()
    {
        // Load từ PlayerPrefs
        savedMasterVol = PlayerPrefs.GetFloat("MasterVolValue", 0f);
        savedMusicVol = PlayerPrefs.GetFloat("MusicVolValue", 0f);
        savedSfxVol = PlayerPrefs.GetFloat("SfxVolValue", 0f);

        // Đặt giá trị cho slider
        masterVol.value = savedMasterVol;
        musicVol.value = savedMusicVol;
        sfxVol.value = savedSfxVol;

        // Đặt giá trị tạm bằng giá trị đã lưu
        tempMasterVol = savedMasterVol;
        tempMusicVol = savedMusicVol;
        tempSfxVol = savedSfxVol;

        // Apply vào AudioMixer
        mainAudioMixer.SetFloat("MasterVol", savedMasterVol);
        mainAudioMixer.SetFloat("MusicVol", savedMusicVol);
        mainAudioMixer.SetFloat("SfxVol", savedSfxVol);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateVolume("MasterVol", savedMasterVol);
            AudioManager.Instance.UpdateVolume("MusicVol", savedMusicVol);
            AudioManager.Instance.UpdateVolume("SfxVol", savedSfxVol);
        }

        hasUnsavedChanges = false;

        Debug.Log($"📊 Loaded: Master={savedMasterVol}, Music={savedMusicVol}, SFX={savedSfxVol}");
    }

    // === HÀM TIỆN ÍCH: MỞ PANEL ===
    public void OpenPanel()
    {
        settingsPanel.SetActive(true);
        LoadSettings(); // Load lại giá trị khi mở
    }
}