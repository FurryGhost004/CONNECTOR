using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public Tilemap groundTilemap;
    public List<Tilemap> forbiddenTilemaps;
    public List<GameObject> enemyPrefabs;
    public int enemyCount = 5;

    [Header("Player Settings")]
    public Transform playerTransform;       // Kéo thả Player vào đây
    public int safeDistanceInCells = 7;    // Khoảng cách an toàn (7 ô)

    public void Initialize(QuestData data)
    {

        enemyPrefabs = data.enemyPrefabs;
        enemyCount = data.enemyCount;
        SpawnEnemiesInMap();
    }
    void Start()
    {
        SpawnEnemiesInMap();
    }

    void SpawnEnemiesInMap()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Chưa gán PlayerTransform vào Spawner!");
            return;
        }

        BoundsInt bounds = groundTilemap.cellBounds;

        // Lấy tọa độ ô của Player
        Vector3Int playerCell = groundTilemap.WorldToCell(playerTransform.position);

        int spawned = 0;
        int safety = 200; // Tăng safety lên vì có thêm điều kiện khoảng cách

        while (spawned < enemyCount && safety > 0)
        {
            safety--;

            Vector3Int cell = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );

            // 1. Kiểm tra phải có gạch nền
            if (!groundTilemap.HasTile(cell)) continue;

            // 2. Kiểm tra khoảng cách với Player (Tính theo ô)
            // Sử dụng Vector3Int.Distance hoặc tính trị tuyệt đối hiệu tọa độ
            float distanceToPlayer = Vector3Int.Distance(cell, playerCell);
            if (distanceToPlayer < safeDistanceInCells) continue;

            // 3. Kiểm tra xem ô này có nằm trong bất kỳ Tilemap cấm nào không
            bool isForbidden = false;
            foreach (Tilemap map in forbiddenTilemaps)
            {
                if (map != null && map.HasTile(cell))
                {
                    isForbidden = true;
                    break;
                }
            }
            if (isForbidden) continue;

            // 4. Thực hiện Spawn
            GameObject selectedEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Vector3 worldPos = groundTilemap.CellToWorld(cell) + groundTilemap.tileAnchor;
            Instantiate(selectedEnemy, worldPos, Quaternion.identity);

            spawned++;
        }
    }
}