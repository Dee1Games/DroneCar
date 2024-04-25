using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform monsterPoint;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform gatesParent;
    
    [SerializeField] private Vector2 gateCount;
    [SerializeField] private float obstacleChance;

    private List<GateObstacle> gates;

    public void Init()
    {
        int count = UnityEngine.Random.Range((int)gateCount.x, (int)gateCount.y+1);
        var allGates = gatesParent.GetComponentsInChildren<GateObstacle>(true).ToList().OrderBy(i => Guid.NewGuid()).ToList();
        gates = new List<GateObstacle>(count);
        for (int i=0 ; i<count ; i++)
        {
            gates.Add(allGates[i]);
        }
        for (int i=count ; i<allGates.Count; i++)
        {
            allGates[i].gameObject.SetActive(false);
        }

        gates = gates.OrderBy(i => -i.transform.position.z).ToList();
        for (int i=0 ; i<count ; i++)
        {
            gates[i].gameObject.name = i.ToString();

            GateObstacle.GateObstacleType t = GateObstacle.GateObstacleType.Obstacle;
            float rnd = UnityEngine.Random.Range(0f, 1f);
            if (rnd > obstacleChance)
            {
                int x = i%3;
                if (x == 0)
                {
                    t = GateObstacle.GateObstacleType.BombGate;
                } else if (x == 1)
                {
                    t = GateObstacle.GateObstacleType.GunGate;
                } else if (x == 2)
                {
                    t = GateObstacle.GateObstacleType.TurboGate;
                } 
            }
            gates[i].Set(t);
            gates[i].Init();
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
