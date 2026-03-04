using System;
using System.Collections.Generic;
using UnityEngine;
using static NPCManager;
public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    [System.Serializable]
    public struct NPCGroup
    {
        public int afterMissionIndex;
        public GameObject[] npcPrefabs;
        public Vector3[] specificSpawnPositions;
    }

    public List<NPCGroup> npcGroups;
    public Transform[] spawnPoints;
    private List<GameObject> currentNPCs = new List<GameObject>();
    void Awake()
    {
        Instance = this;
    }

    public void RefreshNPCs()
    {          
        if (currentNPCs != null)
        {
            foreach (var npc in currentNPCs) 
            {
  
                Destroy(npc);
                Debug.Log("Destroyed NPC: " + npc.name);
            }
             currentNPCs.Clear();
        }
        else
        {
            currentNPCs = new List<GameObject>();

        }

        int highestIndex = PlayerPrefs.GetInt("HighestMissionIndex", 0);
        NPCGroup selectedGroup = new NPCGroup();
        bool groupFound = false;
        for (int i = npcGroups.Count -1; i >= 0; i--)
        {
            if (highestIndex >= npcGroups[i].afterMissionIndex)
            {
                selectedGroup = npcGroups[i];
                groupFound = true;
                break;
            }
        }


            for (int i = 0; i < selectedGroup.npcPrefabs.Length; i++)
            {
                if (selectedGroup.npcPrefabs[i] == null) 
                { continue; }
                Vector3 spawnPos;
                if (selectedGroup.specificSpawnPositions != null && i < selectedGroup.specificSpawnPositions.Length)
                {
                    spawnPos = selectedGroup.specificSpawnPositions[i];
                }
                else if (spawnPoints != null && i < spawnPoints.Length)
                {
                    spawnPos = spawnPoints[i].position;
                }
                else
                {
                    spawnPos = Vector3.zero; // Vị trí dự phòng cuối cùng
                }
                GameObject npc = Instantiate(selectedGroup.npcPrefabs[i], spawnPos, Quaternion.identity);
                currentNPCs.Add(npc);
            }
        }
    }

