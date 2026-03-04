using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public List<Wire> Wires;

    void ShuffleWires()
    {
        List<Vector3> endWirePositions = new List<Vector3>();
        foreach (Wire w in Wires)
        {
            Vector3 pos = w.EndWire.position;
            endWirePositions.Add(pos);
        }
        foreach (Wire w in Wires)
        {
            int randomIndex = Random.Range(0, endWirePositions.Count);
            w.EndWire.position = endWirePositions[randomIndex];
            endWirePositions.RemoveAt(randomIndex);

        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShuffleWires();
    }

    // Update is called once per frame
    void Update()
    {
        int connectedWires = 0;
        foreach (Wire w in Wires)
        {
            if (w.IsConnected())
            {
                connectedWires++;

            }
        }
        if (connectedWires == Wires.Count) //Điều kiện thắng 
        {
            ResetWires();
        }
    }

    public void ResetWires()
    {
        foreach (Wire w in Wires)
        {
            w.SetConnected(false);
        }
        ShuffleWires();
    }
}
