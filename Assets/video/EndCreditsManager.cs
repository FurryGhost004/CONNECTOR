using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EndCreditsManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject creditsPanel;

    [Header("Video Player")]
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;

    [Header("Buttons")]
    public Button closeButton;

    void Start()
    {
        // Ẩn panel ban đầu
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        // Setup video player
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        // Setup close button
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCredits);
    }

    // ═══════════════════════════════════════════════════════════════
    // SHOW CREDITS
    // ═══════════════════════════════════════════════════════════════

    public void ShowCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }

        // Chuẩn bị và phát video
        if (videoPlayer != null)
        {
            videoPlayer.Prepare();
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // CLOSE CREDITS
    // ═══════════════════════════════════════════════════════════════

    public void CloseCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }

        // Dừng video
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // VIDEO EVENTS
    // ═══════════════════════════════════════════════════════════════

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Video hết, tự động đóng (tùy chọn)
        // CloseCredits();
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}