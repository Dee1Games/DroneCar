using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinParticlePool : MonoBehaviour
{
    public static CoinParticlePool Instance;
    
    [SerializeField] private int poolCount;
    [SerializeField] private CoinParticle prefab;

    private List<CoinParticle> list;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        list = new List<CoinParticle>();
        CoinParticle[] preList = transform.GetComponentsInChildren<CoinParticle>();
        int preCount = preList.Length;
        for (int i = 0; i < preCount; i++)
        {
            list.Add(preList[i]);
            preList[i].gameObject.SetActive(false);
        }
        if (poolCount > preCount)
        {
            for (int i = 0; i < poolCount - preCount; i++)
            {
                GameObject go = Instantiate(prefab, transform).gameObject;
                list.Add(go.GetComponent<CoinParticle>());
                go.SetActive(false);
            }
        }
    }

    public void ShowOne(Transform parent, int amount)
    {
        CoinParticle d = list.FirstOrDefault(x => !x.gameObject.activeSelf);
        if (d != null)
        {
            d.gameObject.SetActive(true);
            d.transform.position = parent.position;
            d.transform.parent = parent;
            d.Init(amount);
        }
    } 
}
