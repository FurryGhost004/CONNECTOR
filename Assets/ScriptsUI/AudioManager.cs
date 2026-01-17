using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource; // Nhạc nền
    public AudioSource sfxSource;   // Sound effects

    [Header("Audio Clips - Music cho từng Scene")]
    public AudioClip mainMenuMusic;
    public AudioClip mapMusic;
    public AudioClip gameSceneMusic;
    public AudioClip model01Music;
    public AudioClip settingMusic;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    public static AudioManager Instance;
    private string currentSceneName = "";

    void Awake()
    {
        // SINGLETON PATTERN - Chỉ cho phép 1 AudioManager tồn tại
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Không bị xóa khi chuyển scene

            // ⭐ Setup AudioSource SAU KHI DontDestroyOnLoad
            SetupAudioSources();
        }
        else
        {
            Destroy(gameObject); // Nếu đã có rồi thì xóa cái mới
            return;
        }
    }

    // ⭐ HÀM RIÊNG ĐỂ SETUP AUDIO SOURCES
    void SetupAudioSources()
    {
        // Tìm hoặc tạo Music Source
        if (musicSource == null)
        {
            musicSource = gameObject.GetComponent<AudioSource>();
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Tìm hoặc tạo SFX Source (phải là AudioSource thứ 2)
        if (sfxSource == null)
        {
            AudioSource[] sources = gameObject.GetComponents<AudioSource>();
            if (sources.Length >= 2)
            {
                sfxSource = sources[1];
            }
            else
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Cấu hình cho Music Source
        if (musicSource != null)
        {
            musicSource.loop = true; // Lặp lại mãi
            musicSource.playOnAwake = false;
        }

        // Cấu hình cho SFX Source
        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        Debug.Log("✅ AudioSources đã được setup!");
    }

    void Start()
    {
        // Load settings từ PlayerPrefs
        LoadAudioSettings();

        // Lắng nghe sự kiện chuyển scene
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Phát nhạc cho scene hiện tại
        currentSceneName = SceneManager.GetActiveScene().name;
        PlayMusicForScene(currentSceneName);

        Debug.Log("AudioManager Start() - Phát nhạc cho scene: " + currentSceneName);
    }

    void OnDestroy()
    {
        // Hủy đăng ký sự kiện
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Được gọi mỗi khi chuyển scene
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;

        // Chỉ đổi nhạc nếu scene THỰC SỰ khác
        if (newSceneName != currentSceneName)
        {
            currentSceneName = newSceneName;
            PlayMusicForScene(newSceneName);
            Debug.Log($"🎵 Chuyển scene → Đổi nhạc: {newSceneName}");
        }
    }

    void Update()
    {
        // Debug: Nhấn M để kiểm tra trạng thái
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("=== AUDIO MANAGER STATUS ===");
            Debug.Log($"Scene hiện tại: {currentSceneName}");
            Debug.Log($"AudioManager Instance: {Instance != null}");
            Debug.Log($"Music đang phát: {musicSource != null && musicSource.isPlaying}");
            Debug.Log($"Clip: {(musicSource?.clip != null ? musicSource.clip.name : "NULL")}");
            Debug.Log($"Time: {(musicSource != null && musicSource.clip != null ? $"{musicSource.time:F2}s / {musicSource.clip.length:F2}s" : "NULL")}");
            Debug.Log($"Volume: {(musicSource != null ? musicSource.volume.ToString() : "NULL")}");

            // Thử phát lại nhạc nếu bị dừng
            if (musicSource != null && !musicSource.isPlaying)
            {
                Debug.Log("⚠️ Nhạc không chạy! Đang thử phát lại...");
                PlayMusicForScene(currentSceneName);
            }
        }
    }

    // === PHÁT NHẠC THEO SCENE ===
    void PlayMusicForScene(string sceneName)
    {
        // ⭐ KIỂM TRA NULL TRƯỚC KHI DÙNG
        if (musicSource == null)
        {
            Debug.LogError("❌ Music Source đã bị destroy! Đang tạo lại...");
            SetupAudioSources();
            return;
        }

        // ⭐ NẾU NHẠC ĐANG PHÁT → KHÔNG LÀM GÌ CẢ
        if (musicSource.isPlaying)
        {
            Debug.Log($"ℹ️ Nhạc đang phát, giữ nguyên khi chuyển scene: {sceneName}");
            return; // ← THOÁT LUÔN, KHÔNG ĐỔI NHẠC
        }

        // ⭐ CHỈ PHÁT NHẠC NẾU CHƯA CÓ NHẠC NÀO (lần đầu tiên)
        if (mainMenuMusic != null)
        {
            musicSource.clip = mainMenuMusic;
            musicSource.Play();
            Debug.Log($"✅ Phát nhạc lần đầu: {mainMenuMusic.name}");
        }
        else
        {
            Debug.LogError("❌ Chưa gán nhạc vào Main Menu Music!");
        }
    }

    // === PHÁT NHẠC NỀN (Dùng cho manual call) ===
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("❌ Clip NULL!");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogError("❌ Music Source NULL!");
            return;
        }

        // Chỉ đổi nếu khác clip hiện tại
        if (musicSource.clip != clip)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
            Debug.Log($"✅ Đang phát nhạc: {clip.name}");
        }
    }

    // === DỪNG NHẠC NỀN ===
    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    // === PHÁT SOUND EFFECT ===
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // === LOAD SETTINGS TỪ PLAYERPREFS ===
    public void LoadAudioSettings()
    {
        // Load volume từ PlayerPrefs (giá trị từ -80 đến 0)
        float masterVol = PlayerPrefs.GetFloat("MasterVolValue", 0f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolValue", 0f);
        float sfxVol = PlayerPrefs.GetFloat("SfxVolValue", 0f);

        Debug.Log($"📊 Loaded volumes: Master={masterVol}, Music={musicVol}, SFX={sfxVol}");

        // Apply vào AudioMixer
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVol", masterVol);
            audioMixer.SetFloat("MusicVol", musicVol);
            audioMixer.SetFloat("SfxVol", sfxVol);
        }
    }

    // === CẬP NHẬT VOLUME REAL-TIME (Gọi từ SettingManager) ===
    public void UpdateVolume(string parameterName, float value)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(parameterName, value);
        }
    }
}