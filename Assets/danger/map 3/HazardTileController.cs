using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class HazardTileController : MonoBehaviour
{
    [Header("Hazard Tilemap")]
    [Tooltip("Kéo Hazard Tilemap vào đây")]
    public Tilemap hazardTilemap;

    [Header("Tiles")]
    [Tooltip("Tile khi an toàn (giống sàn bình thường)")]
    public TileBase safeTile;

    [Tooltip("Tile cảnh báo (nhấp nháy vàng)")]
    public TileBase warningTile;

    [Tooltip("Tile nguy hiểm (lava/hố)")]
    public TileBase dangerTile;

    [Header("Timing Settings")]
    [Tooltip("Thời gian tối thiểu hazard ĐÓNG (giây)")]
    public float minCloseTime = 2f;

    [Tooltip("Thời gian tối đa hazard ĐÓNG (giây)")]
    public float maxCloseTime = 5f;

    [Tooltip("Thời gian cảnh báo trước khi mở (giây)")]
    public float warningDuration = 0.5f;

    [Tooltip("Thời gian hazard MỞ (nguy hiểm) (giây)")]
    public float dangerDuration = 2f;

    [Header("Damage Settings")]
    [Tooltip("Lượng máu mất khi rơi vào hazard")]
    public int damageAmount = 30;

    [Header("Visual Effects")]
    [Tooltip("Tốc độ nhấp nháy cảnh báo")]
    public float blinkSpeed = 0.1f;

    [Header("⚡ INSTANT ACTIVATION")]
    [Tooltip("⚡ DELAY CỰC NGẮN GIỮA MỖI TILE (0.01-0.05s = gần như cùng lúc)")]
    public float tileActivationDelay = 0.02f; // ✅ 0.02s = 20ms

    [Header("⚡ RANDOM SELECTION")]
    [Tooltip("% tiles được kích hoạt (50% = chỉ bật 1 nửa, 70% = bật 70%)")]
    [Range(0.1f, 1f)]
    public float activationPercentage = 0.6f; // ✅ MẶC ĐỊNH BẬT 60% = khoảng 22-24/38 tiles

    [Tooltip("Random lại tiles mỗi lần activate?")]
    public bool randomizeEachTime = true;

    [Header("Debug")]
    public bool showDebugLogs = false;

    private Tilemap colliderTilemap;
    private TilemapCollider2D hazardCollider;
    private List<Vector3Int> hazardPositions = new List<Vector3Int>();

    private bool isActivated = false;

    // ✅ DANH SÁCH TILES ĐANG ACTIVE
    private List<Vector3Int> activeTiles = new List<Vector3Int>();

    void Start()
    {
        if (hazardTilemap == null)
        {
            Debug.LogError("⚠️ Hazard Tilemap chưa gán!");
            enabled = false;
            return;
        }

        if (safeTile == null || warningTile == null || dangerTile == null)
        {
            Debug.LogError("⚠️ Thiếu Tiles! Gán Safe/Warning/Danger Tile trong Inspector!");
            enabled = false;
            return;
        }

        CreateColliderTilemap();
        FindAllHazardTiles();

        if (showDebugLogs)
            Debug.Log($"✅ Tìm thấy {hazardPositions.Count} hazard tiles! (Chưa hoạt động)");
    }

    // ═══════════════════════════════════════════════════════════════
    // ✅ BẬT HAZARD - KÍCH HOẠT GẦN NHƯ ĐỒNG THỜI (RANDOM MỘT PHẦN)
    // ═══════════════════════════════════════════════════════════════
    public void ActivateHazards()
    {
        if (isActivated)
        {
            if (showDebugLogs)
                Debug.Log("⚠️ Hazards đã được activate rồi!");
            return;
        }

        isActivated = true;

        // ✅ CHỌN RANDOM MỘT PHẦN TILES
        SelectRandomTiles();

        if (showDebugLogs)
            Debug.Log($"🔥 HAZARDS ACTIVATED! {activeTiles.Count}/{hazardPositions.Count} tiles được chọn ({activationPercentage * 100}%)");

        // ✅ KÍCH HOẠT INSTANT - GẦN NHƯ CÙNG LÚC
        StartCoroutine(InstantActivateAll());
    }

    // ═══════════════════════════════════════════════════════════════
    // ✅ TẮT HAZARD
    // ═══════════════════════════════════════════════════════════════
    public void DeactivateHazards()
    {
        if (!isActivated)
        {
            if (showDebugLogs)
                Debug.Log("⚠️ Hazards chưa được activate!");
            return;
        }

        isActivated = false;
        StopAllCoroutines();

        // Reset tất cả tiles đang active về safe
        foreach (Vector3Int pos in activeTiles)
        {
            hazardTilemap.SetTile(pos, safeTile);
            SetTileCollider(pos, false);
        }

        if (showDebugLogs)
            Debug.Log($"⬜ Hazards đã TẮT! ({activeTiles.Count} tiles reset)");
    }

    // ═══════════════════════════════════════════════════════════════
    // ✅ CHỌN RANDOM MỘT PHẦN TILES
    // ═══════════════════════════════════════════════════════════════
    void SelectRandomTiles()
    {
        activeTiles.Clear();

        // Tạo list tạm để shuffle
        List<Vector3Int> shuffled = new List<Vector3Int>(hazardPositions);

        // Shuffle (Fisher-Yates)
        for (int i = 0; i < shuffled.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffled.Count);
            Vector3Int temp = shuffled[i];
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;
        }

        // Lấy % tiles
        int count = Mathf.RoundToInt(hazardPositions.Count * activationPercentage);
        count = Mathf.Max(1, count); // Ít nhất 1 tile

        for (int i = 0; i < count; i++)
        {
            activeTiles.Add(shuffled[i]);
        }

        if (showDebugLogs)
            Debug.Log($"🎲 Random: {activeTiles.Count}/{hazardPositions.Count} tiles được chọn");
    }

    void CreateColliderTilemap()
    {
        GameObject colliderObj = new GameObject("HazardCollider_Runtime");

        Grid grid = hazardTilemap.GetComponentInParent<Grid>();
        if (grid != null)
        {
            colliderObj.transform.SetParent(grid.transform);
        }
        else
        {
            colliderObj.transform.SetParent(hazardTilemap.transform.parent);
        }

        colliderObj.transform.localPosition = Vector3.zero;
        colliderObj.layer = hazardTilemap.gameObject.layer;
        colliderObj.tag = "Hazard";

        colliderTilemap = colliderObj.AddComponent<Tilemap>();
        TilemapRenderer renderer = colliderObj.AddComponent<TilemapRenderer>();
        renderer.enabled = false;

        hazardCollider = colliderObj.AddComponent<TilemapCollider2D>();
        hazardCollider.isTrigger = true;

        if (showDebugLogs)
            Debug.Log("✅ Đã tạo Tilemap Collider ẩn!");
    }

    void FindAllHazardTiles()
    {
        hazardPositions.Clear();

        BoundsInt bounds = hazardTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (hazardTilemap.HasTile(pos))
                {
                    hazardPositions.Add(pos);
                    hazardTilemap.SetTile(pos, safeTile);
                }
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // ✅ KÍCH HOẠT TẤT CẢ GẦN NHƯ CÙNG LÚC (delay 0.02s = 20ms)
    // ═══════════════════════════════════════════════════════════════
    IEnumerator InstantActivateAll()
    {
        int count = 0;

        // ✅ CHỈ BẬT TILES TRONG activeTiles (đã random)
        foreach (Vector3Int pos in activeTiles)
        {
            // ✅ BẮT ĐẦU CYCLE CHO TILE NÀY NGAY
            StartCoroutine(HazardCycle(pos));

            count++;

            // ✅ DELAY CỰC NGẮN (0.02s) ĐỂ TRÁNH LAG
            if (count % 5 == 0) // Mỗi 5 tiles delay 1 chút
            {
                yield return new WaitForSeconds(tileActivationDelay);
            }
        }

        if (showDebugLogs)
            Debug.Log($"🔥 ĐÃ KÍCH HOẠT {count} TILES!");
    }

    // ═══════════════════════════════════════════════════════════════
    // CYCLE MỘT TILE: AN TOÀN → CẢNH BÁO → NGUY HIỂM → LẶP LẠI
    // ═══════════════════════════════════════════════════════════════
    IEnumerator HazardCycle(Vector3Int pos)
    {
        // ✅ RANDOM OFFSET BAN ĐẦU ĐỂ KHÔNG ĐỒNG BỘ 100%
        yield return new WaitForSeconds(Random.Range(0f, 0.5f));

        while (isActivated)
        {
            // ─────────────────────────────────────────────────────────
            // GIAI ĐOẠN 1: AN TOÀN
            // ─────────────────────────────────────────────────────────
            hazardTilemap.SetTile(pos, safeTile);
            SetTileCollider(pos, false);

            float safeTime = Random.Range(minCloseTime, maxCloseTime); // ✅ DÙNG TRỰC TIẾP
            yield return new WaitForSeconds(safeTime);

            // ─────────────────────────────────────────────────────────
            // GIAI ĐOẠN 2: CẢNH BÁO (Nhấp nháy)
            // ─────────────────────────────────────────────────────────
            float warningElapsed = 0f;
            bool showWarning = true;

            while (warningElapsed < warningDuration)
            {
                hazardTilemap.SetTile(pos, showWarning ? warningTile : safeTile);
                showWarning = !showWarning;

                yield return new WaitForSeconds(blinkSpeed);
                warningElapsed += blinkSpeed;
            }

            // ─────────────────────────────────────────────────────────
            // GIAI ĐOẠN 3: NGUY HIỂM
            // ─────────────────────────────────────────────────────────
            hazardTilemap.SetTile(pos, dangerTile);
            SetTileCollider(pos, true);

            yield return new WaitForSeconds(dangerDuration); // ✅ DÙNG TRỰC TIẾP
        }

        // ✅ KHI TẮT, RESET VỀ SAFE
        hazardTilemap.SetTile(pos, safeTile);
        SetTileCollider(pos, false);
    }

    void SetTileCollider(Vector3Int pos, bool enabled)
    {
        if (colliderTilemap == null) return;

        if (enabled)
        {
            colliderTilemap.SetTile(pos, dangerTile);
        }
        else
        {
            colliderTilemap.SetTile(pos, null);
        }
    }

    public int GetDamageAmount()
    {
        return damageAmount;
    }
}