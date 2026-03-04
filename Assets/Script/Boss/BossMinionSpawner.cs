using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossMinionSpawner : MonoBehaviour
{
    [Header("Minion Prefabs (3 loại quái)")]
    public GameObject[] minionPrefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 5f;
    public int spawnAmount = 2;
    public int maxMinionsAlive = 10;

    [Header("Spawn Area")]
    public float spawnRadius = 4f;

    private List<GameObject> aliveMinions = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            CleanupList();

            if (aliveMinions.Count < maxMinionsAlive)
            {
                SpawnMinions();
            }
        }
    }

    void SpawnMinions()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            if (aliveMinions.Count >= maxMinionsAlive)
                return;

            // Random prefab
            int randomIndex = Random.Range(0, minionPrefabs.Length);
            GameObject prefab = minionPrefabs[randomIndex];

            // Random vị trí quanh boss
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);

            GameObject minion = Instantiate(prefab, spawnPos, Quaternion.identity);

            aliveMinions.Add(minion);
        }
    }

    void CleanupList()
    {
        aliveMinions.RemoveAll(minion => minion == null);
    }
}