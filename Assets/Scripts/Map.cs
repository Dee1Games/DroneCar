using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform monsterPoint;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform gatesParent;

    private GateObstacle[] gates;

    public void Init()
    {
        gates = gatesParent.GetComponentsInChildren<GateObstacle>();
        foreach (GateObstacle gate in gates)
        {
            gate.Init();
        }
    }
    
    public void Refresh()
    {
        foreach (GateObstacle gate in gates)
        {
            gate.Refresh();
        }
    }

    public Transform GetMonsterParent()
    {
        return monsterPoint;
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
    }
}
