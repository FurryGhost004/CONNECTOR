using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HazardPatternController : MonoBehaviour
{
    [Header("Hazard Tilemap")]
    public Tilemap hazardTilemap;

    [Header("Tiles")]
    public TileBase safeTile;
    public TileBase warningTile;
    public TileBase dangerTile;

    [Header("⚡ PATTERN ENABLE/DISABLE")]
    [Tooltip("✅ Tick để bật pattern này")]
    public bool enableCrossPattern = true;

    [Tooltip("✅ Tick để bật pattern này")]
    public bool enableDiagonalPattern = true;

    [Tooltip("✅ Tick để bật pattern này")]
    public bool enableChaosPattern = true;

    [Header("⚡ PATTERN WEIGHTS (Tỷ lệ xuất hiện khi BẬT)")]
    [Tooltip("Tỷ lệ xuất hiện Cross (0 = không bao giờ, 100 = nhiều nhất)")]
    [Range(0, 100)]
    public int crossPatternWeight = 33;

    [Tooltip("Tỷ lệ xuất hiện Diagonal (0 = không bao giờ, 100 = nhiều nhất)")]
    [Range(0, 100)]
    public int diagonalPatternWeight = 33;

    [Tooltip("Tỷ lệ xuất hiện Chaos (0 = không bao giờ, 100 = nhiều nhất)")]
    [Range(0, 100)]
    public int chaosPatternWeight = 34;

    [Header("⚡ TIMING SETTINGS")]
    [Tooltip("Thời gian cảnh báo (giây)")]
    public float warningDuration = 0.8f;

    [Tooltip("Thời gian nguy hiểm (giây)")]
    public float dangerDuration = 1.5f;

    [Tooltip("Thời gian nghỉ giữa các pattern (giây)")]
    public float cooldownDuration = 0.5f;

    [Tooltip("Tốc độ nhấp nháy")]
    public float blinkSpeed = 0.1f;

    [Header("⚡ CROSS PATTERN SETTINGS")]
    [Tooltip("Độ dày chữ thập (1 = mỏng, 10 = siêu dày)")]
    [Range(1, 10)]
    public int crossThickness = 3;

    [Tooltip("Số chữ thập xuất hiện cùng lúc (1 = 1 cái, 5 = 5 cái)")]
    [Range(1, 10)]
    public int crossCount = 1;

    [Header("⚡ DIAGONAL PATTERN SETTINGS")]
    [Tooltip("Độ dày đường chéo (1 = mỏng, 5 = dày)")]
    [Range(1, 5)]
    public int diagonalThickness = 1;

    [Tooltip("Kiểu chéo: Both = cả 2, Slash = /, Backslash = \\")]
    public DiagonalType diagonalType = DiagonalType.Both;

    [Header("⚡ CHAOS SETTINGS")]
    [Tooltip("% tiles chaos (0.5 = 50%, 0.9 = 90%)")]
    [Range(0.3f, 1f)]
    public float chaosPercentage = 0.8f;

    [Tooltip("Dùng block 2x2 (để player cao 2 ô không bị dame)")]
    public bool useBlock2x2 = true;

    [Tooltip("Nếu không dùng 2x2, thì độ phân tán (1 = dày đặc, 3 = thưa)")]
    [Range(1, 5)]
    public int chaosSpacing = 1;

    [Header("Damage Settings")]
    public int damageAmount = 30;

    [Header("Debug")]
    public bool showDebugLogs = false;

    public enum DiagonalType
    {
        Slash,       // /
        Backslash,   // \
        Both         // Cả 2
    }

    private Tilemap colliderTilemap;
    private TilemapCollider2D hazardCollider;

    private Dictionary<int, List<Vector3Int>> rowTiles = new Dictionary<int, List<Vector3Int>>();
    private Dictionary<int, List<Vector3Int>> columnTiles = new Dictionary<int, List<Vector3Int>>();
    private List<Vector3Int> allTiles = new List<Vector3Int>();

    private List<string> enabledPatterns = new List<string>();

    void Start()
    {
        if (hazardTilemap == null || safeTile == null || warningTile == null || dangerTile == null)
        {
            Debug.LogError("⚠️ Thiếu Tilemap hoặc Tiles!");
            enabled = false;
            return;
        }

        CreateColliderTilemap();
        FindAllHazardTiles();
        UpdateEnabledPatterns();

        if (enabledPatterns.Count == 0)
        {
            Debug.LogError("⚠️ Không có pattern nào được bật! Tick ít nhất 1 pattern!");
            enabled = false;
            return;
        }

        StartCoroutine(PatternLoop());

        if (showDebugLogs)
            Debug.Log($"✅ Tiles: {allTiles.Count} | Patterns: {string.Join(", ", enabledPatterns)}");
    }

    void UpdateEnabledPatterns()
    {
        enabledPatterns.Clear();

        if (enableCrossPattern) enabledPatterns.Add("Cross");
        if (enableDiagonalPattern) enabledPatterns.Add("Diagonal");
        if (enableChaosPattern) enabledPatterns.Add("Chaos");
    }

    void CreateColliderTilemap()
    {
        GameObject colliderObj = new GameObject("HazardCollider_Runtime");

        Grid grid = hazardTilemap.GetComponentInParent<Grid>();
        if (grid != null)
            colliderObj.transform.SetParent(grid.transform);
        else
            colliderObj.transform.SetParent(hazardTilemap.transform.parent);

        colliderObj.transform.localPosition = Vector3.zero;
        colliderObj.layer = hazardTilemap.gameObject.layer;
        colliderObj.tag = "Hazard";

        colliderTilemap = colliderObj.AddComponent<Tilemap>();
        TilemapRenderer renderer = colliderObj.AddComponent<TilemapRenderer>();
        renderer.enabled = false;

        hazardCollider = colliderObj.AddComponent<TilemapCollider2D>();
        hazardCollider.isTrigger = true;
    }

    void FindAllHazardTiles()
    {
        rowTiles.Clear();
        columnTiles.Clear();
        allTiles.Clear();

        BoundsInt bounds = hazardTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (hazardTilemap.HasTile(pos))
                {
                    allTiles.Add(pos);

                    if (!rowTiles.ContainsKey(y))
                        rowTiles[y] = new List<Vector3Int>();
                    rowTiles[y].Add(pos);

                    if (!columnTiles.ContainsKey(x))
                        columnTiles[x] = new List<Vector3Int>();
                    columnTiles[x].Add(pos);

                    hazardTilemap.SetTile(pos, safeTile);
                }
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // VÒNG LẶP PATTERN
    // ═══════════════════════════════════════════════════════════════
    IEnumerator PatternLoop()
    {
        while (true)
        {
            string selectedPattern = GetWeightedRandomPattern();

            if (showDebugLogs)
                Debug.Log($"🎲 Pattern: {selectedPattern}");

            switch (selectedPattern)
            {
                case "Cross":
                    yield return StartCoroutine(AttackCrossPattern());
                    break;

                case "Diagonal":
                    yield return StartCoroutine(AttackDiagonalPattern());
                    break;

                case "Chaos":
                    yield return StartCoroutine(AttackChaosPattern());
                    break;
            }

            yield return new WaitForSeconds(cooldownDuration);
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // WEIGHTED RANDOM (CHỈ CHỌN TRONG CÁC PATTERN ĐƯỢC BẬT)
    // ═══════════════════════════════════════════════════════════════
    string GetWeightedRandomPattern()
    {
        List<string> patterns = new List<string>();
        List<int> weights = new List<int>();

        if (enableCrossPattern)
        {
            patterns.Add("Cross");
            weights.Add(crossPatternWeight);
        }

        if (enableDiagonalPattern)
        {
            patterns.Add("Diagonal");
            weights.Add(diagonalPatternWeight);
        }

        if (enableChaosPattern)
        {
            patterns.Add("Chaos");
            weights.Add(chaosPatternWeight);
        }

        if (patterns.Count == 0)
            return "Chaos"; // Fallback

        // Calculate total weight
        int totalWeight = 0;
        foreach (int w in weights)
            totalWeight += w;

        if (totalWeight == 0)
            return patterns[Random.Range(0, patterns.Count)];

        // Weighted random
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        for (int i = 0; i < patterns.Count; i++)
        {
            currentWeight += weights[i];
            if (randomValue < currentWeight)
                return patterns[i];
        }

        return patterns[patterns.Count - 1];
    }

    // ═══════════════════════════════════════════════════════════════
    // 1. CROSS PATTERN - Chữ thập dày
    // ═══════════════════════════════════════════════════════════════
    IEnumerator AttackCrossPattern()
    {
        List<Vector3Int> crossTiles = new List<Vector3Int>();

        for (int i = 0; i < crossCount; i++)
        {
            List<int> rows = rowTiles.Keys.ToList();
            List<int> cols = columnTiles.Keys.ToList();

            int centerRow = rows[Random.Range(0, rows.Count)];
            int centerCol = cols[Random.Range(0, cols.Count)];

            // Thêm hàng/cột trung tâm + các hàng/cột xung quanh
            for (int j = -crossThickness / 2; j <= crossThickness / 2; j++)
            {
                int targetRow = centerRow + j;
                int targetCol = centerCol + j;

                if (rowTiles.ContainsKey(targetRow))
                    crossTiles.AddRange(rowTiles[targetRow]);

                if (columnTiles.ContainsKey(targetCol))
                    crossTiles.AddRange(columnTiles[targetCol]);
            }
        }

        crossTiles = crossTiles.Distinct().ToList();

        yield return StartCoroutine(AttackLine(crossTiles, $"CHỮ THẬP x{crossCount} (Độ dày {crossThickness}) - {crossTiles.Count} tiles"));
    }

    // ═══════════════════════════════════════════════════════════════
    // 2. DIAGONAL PATTERN - Đường chéo
    // ═══════════════════════════════════════════════════════════════
    IEnumerator AttackDiagonalPattern()
    {
        List<Vector3Int> diagonalTiles = new List<Vector3Int>();
        string patternName = "";

        bool useSlash = false;
        bool useBackslash = false;

        switch (diagonalType)
        {
            case DiagonalType.Slash:
                useSlash = true;
                patternName = "CHÉO /";
                break;
            case DiagonalType.Backslash:
                useBackslash = true;
                patternName = "CHÉO \\";
                break;
            case DiagonalType.Both:
                useSlash = true;
                useBackslash = true;
                patternName = "CHÉO X";
                break;
        }

        foreach (Vector3Int tile in allTiles)
        {
            bool addTile = false;

            if (useSlash)
            {
                // Chéo / - độ dày
                for (int i = 0; i < diagonalThickness; i++)
                {
                    if ((tile.x + tile.y + i) % (diagonalThickness * 2) < diagonalThickness)
                    {
                        addTile = true;
                        break;
                    }
                }
            }

            if (useBackslash && !addTile)
            {
                // Chéo \ - độ dày
                for (int i = 0; i < diagonalThickness; i++)
                {
                    if ((tile.x - tile.y + i) % (diagonalThickness * 2) < diagonalThickness)
                    {
                        addTile = true;
                        break;
                    }
                }
            }

            if (addTile)
                diagonalTiles.Add(tile);
        }

        yield return StartCoroutine(AttackLine(diagonalTiles, $"{patternName} (Độ dày {diagonalThickness}) - {diagonalTiles.Count} tiles"));
    }

    // ═══════════════════════════════════════════════════════════════
    // 3. CHAOS PATTERN - Block 2x2 hoặc Random
    // ═══════════════════════════════════════════════════════════════
    IEnumerator AttackChaosPattern()
    {
        List<Vector3Int> chaosTiles = new List<Vector3Int>();

        if (useBlock2x2)
        {
            // ✅ BÀN CỜ 2x2 (Player cao 2 ô không bị dame)
            chaosTiles = GetBlock2x2Pattern();
        }
        else
        {
            // Random tiles với spacing
            chaosTiles = GetRandomChaosPattern();
        }

        yield return StartCoroutine(AttackLine(chaosTiles, $"CHAOS ({chaosTiles.Count}/{allTiles.Count} tiles - {(chaosTiles.Count * 100f / allTiles.Count):F0}%)"));
    }

    // ═══════════════════════════════════════════════════════════════
    // ✅ TẠO PATTERN 2x2 BLOCK
    // ═══════════════════════════════════════════════════════════════
    List<Vector3Int> GetBlock2x2Pattern()
    {
        List<Vector3Int> blockTiles = new List<Vector3Int>();

        // Duyệt theo block 2x2
        foreach (Vector3Int tile in allTiles)
        {
            // Chỉ lấy tile ở góc trái dưới của mỗi block 2x2
            if (tile.x % 2 == 0 && tile.y % 2 == 0)
            {
                // Random block này có nguy hiểm không
                if (Random.value < chaosPercentage)
                {
                    // Thêm 4 ô trong block 2x2
                    blockTiles.Add(tile);
                    blockTiles.Add(new Vector3Int(tile.x + 1, tile.y, 0));
                    blockTiles.Add(new Vector3Int(tile.x, tile.y + 1, 0));
                    blockTiles.Add(new Vector3Int(tile.x + 1, tile.y + 1, 0));
                }
            }
        }

        // Lọc chỉ lấy tiles tồn tại
        blockTiles = blockTiles.Where(t => allTiles.Contains(t)).ToList();

        return blockTiles;
    }

    // ═══════════════════════════════════════════════════════════════
    // ✅ TẠO PATTERN CHAOS RANDOM VỚI SPACING
    // ═══════════════════════════════════════════════════════════════
    List<Vector3Int> GetRandomChaosPattern()
    {
        List<Vector3Int> chaosTiles = new List<Vector3Int>();

        foreach (Vector3Int tile in allTiles)
        {
            // Spacing: 1 = dày đặc, 2 = cách 1, 3 = cách 2
            if (tile.x % chaosSpacing == 0 && tile.y % chaosSpacing == 0)
            {
                if (Random.value < chaosPercentage)
                {
                    chaosTiles.Add(tile);
                }
            }
        }

        return chaosTiles;
    }

    // ═══════════════════════════════════════════════════════════════
    // ATTACK MỘT DÃNG TILES
    // ═══════════════════════════════════════════════════════════════
    IEnumerator AttackLine(List<Vector3Int> tiles, string lineName)
    {
        // CẢNH BÁO
        if (showDebugLogs)
            Debug.Log($"🟡 {lineName}!");

        float warningElapsed = 0f;
        bool showWarning = true;

        while (warningElapsed < warningDuration)
        {
            foreach (Vector3Int pos in tiles)
            {
                hazardTilemap.SetTile(pos, showWarning ? warningTile : safeTile);
            }

            showWarning = !showWarning;
            yield return new WaitForSeconds(blinkSpeed);
            warningElapsed += blinkSpeed;
        }

        // NGUY HIỂM
        if (showDebugLogs)
            Debug.Log($"🔴 {lineName} NGUY HIỂM!");

        foreach (Vector3Int pos in tiles)
        {
            hazardTilemap.SetTile(pos, dangerTile);
            colliderTilemap.SetTile(pos, dangerTile);
        }

        yield return new WaitForSeconds(dangerDuration);

        // AN TOÀN
        foreach (Vector3Int pos in tiles)
        {
            hazardTilemap.SetTile(pos, safeTile);
            colliderTilemap.SetTile(pos, null);
        }
    }

    public int GetDamageAmount()
    {
        return damageAmount;
    }
}