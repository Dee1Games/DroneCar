using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPath : MonoBehaviour
{
    public Transform Gates;
    [SerializeField] private Transform obstaclesParent;
    [SerializeField] private Coin[] coins;

    public void Init()
    {
        int count = obstaclesParent.childCount;
        int rnd = Random.Range(0, count);

        for (int i = 0; i < count; i++)
        {
            obstaclesParent.GetChild(i).gameObject.SetActive(i==rnd);
        }
        for (int i = 0; i < coins.Length; i++)
        {
            coins[i].Activate();
        }
    }

}
