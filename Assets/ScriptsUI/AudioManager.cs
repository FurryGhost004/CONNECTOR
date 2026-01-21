using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip mapMusic;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    public static AudioManager Instance;

    // =========================
    // SINGLETON
    // =========================
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SetupAudioSources();
    }


    void SetupAudioSources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        if (sources.Length < 2)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            musicSource = sources[0];
            sfxSource = sources[1];
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    // =========================
    // START
    // =========================
    void Start()
    {
        Debug.Log("🎧 AudioManager STARTED: " + gameObject.GetInstanceID());
        LoadAudioSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }


    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // =========================
    // SCENE CHANGE
    // =========================
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    // =========================
    // PLAY MUSIC BY SCENE (FIX)
    // =========================
    void PlayMusicForScene(string sceneName)
    {
        if (musicSource == null) return;

        AudioClip newClip = null;

        // 1️⃣ Chọn nhạc theo scene
        if (sceneName == "MainMenu" || sceneName == "WaittingRoom")
        {
            newClip = mainMenuMusic;
        }
        else
        {
            newClip = mapMusic;
        }

        // 2️⃣ Nếu không có nhạc → im lặng
        if (newClip == null)
        {
            musicSource.Stop();
            musicSource.clip = null;
            return;
        }

        // 3️⃣ Nếu đang phát đúng nhạc thì thôi
        if (musicSource.clip == newClip && musicSource.isPlaying)
            return;

        // 4️⃣ IM LẶNG TUYỆT ĐỐI TRƯỚC KHI ĐỔI
        musicSource.Stop();
        musicSource.clip = null;

        // 5️⃣ Play nhạc mới
        musicSource.clip = newClip;
        musicSource.Play();

        Debug.Log($"🎵 Scene: {sceneName} → Play {newClip.name}");
    }



    // =========================
    // SFX
    // =========================
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    // =========================
    // AUDIO SETTINGS
    // =========================
    public void LoadAudioSettings()
    {
        float master = PlayerPrefs.GetFloat("MasterVolValue", 0f);
        float music = PlayerPrefs.GetFloat("MusicVolValue", 0f);
        float sfx = PlayerPrefs.GetFloat("SfxVolValue", 0f);

        audioMixer.SetFloat("MasterVol", master);
        audioMixer.SetFloat("MusicVol", music);
        audioMixer.SetFloat("SfxVol", sfx);
    }

    public void UpdateVolume(string param, float value)
    {
        audioMixer.SetFloat(param, value);
    }
}
