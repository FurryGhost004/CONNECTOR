using UnityEngine;
using System;

public class VisionCone : MonoBehaviour
{
    [Header("Visual Settings")]
    public SpriteRenderer coneRenderer;
    public Color patrolColor = new Color(1f, 0.92f, 0.016f, 0.3f); // Màu Vàng
    public Color alertColor = new Color(1f, 0f, 0f, 0.5f);        // Màu Đỏ

    public bool canSeePlayer = false;
    private Transform playerTransform;
    private CharaterController playerController;

    public event Action<Transform> OnPlayerSpotted;
    public event Action OnPlayerLost;

    private void Start()
    {
        if (coneRenderer == null) coneRenderer = GetComponent<SpriteRenderer>();
        // Khởi đầu là màu vàng
        SetAlertMode(false);
    }

    // Hàm này để Enemy Controller gọi vào
    public void SetAlertMode(bool isAlert)
    {
        if (coneRenderer != null)
        {
            coneRenderer.color = isAlert ? alertColor : patrolColor;
        }
    }

    private void Update()
    {
        // Logic phát hiện player giữ nguyên
        bool isSeeing = (playerTransform != null && playerController != null && !playerController.isHidden);

        if (isSeeing)
        {
            if (!canSeePlayer)
            {
                canSeePlayer = true;
                OnPlayerSpotted?.Invoke(playerTransform);
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            OnPlayerLost?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            playerController = other.GetComponent<CharaterController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = null;
            playerController = null;
        }
    }
}