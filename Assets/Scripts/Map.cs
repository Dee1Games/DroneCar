using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform monsterPos;
    [SerializeField] private Transform[] spawnPoints;

    public Transform GetMonsterParent()
    {
        return monsterPos;
    }

    public Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
