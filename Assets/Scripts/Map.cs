using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform monsterPoint;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform gatesParent;

    public void Init()
    {
        GateObstacle[] gates = gatesParent.GetComponentsInChildren<GateObstacle>();
        foreach (GateObstacle gate in gates)
        {
            gate.Init();
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
