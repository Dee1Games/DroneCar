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
        if (!UserManager.Instance.Data.SeenMoveTutorial)
        {
            return spawnPoints[0];
        }
        if (!UserManager.Instance.Data.SeenFlyTutorial)
        {
            return spawnPoints[1];
        }
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
