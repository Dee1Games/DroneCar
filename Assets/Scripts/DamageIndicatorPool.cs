using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageIndicatorPool : MonoBehaviour
{
    public static DamageIndicatorPool Instance;
    
    [SerializeField] private int poolCount;
    [SerializeField] private DamageIndicator prefab;

    private List<DamageIndicator> list;

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
        list = new List<DamageIndicator>();
        DamageIndicator[] preList = transform.GetComponentsInChildren<DamageIndicator>();
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
                list.Add(go.GetComponent<DamageIndicator>());
                go.SetActive(false);
            }
        }
    }

    public void ShowOne(Vector3 pos, float damage)
    {
        if (damage <= 0)
            return;
        
        DamageIndicator d = list.FirstOrDefault(x => !x.gameObject.activeSelf);
        if (d != null)
        {
            d.gameObject.SetActive(true);
            d.transform.position = pos;
            d.Init(damage);
        }
    } 
}
