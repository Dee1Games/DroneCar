using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LimbSmokePool : MonoBehaviour
{
    public static LimbSmokePool Instance;
    
    [SerializeField] private int poolCount;

    [SerializeField] private Transform lowParent;
    [SerializeField] private Transform midParent;
    [SerializeField] private Transform highParent;

    [SerializeField] private LimbSmoke prefabLow;
    [SerializeField] private LimbSmoke prefabMid;
    [SerializeField] private LimbSmoke prefabHigh;

    private List<LimbSmoke> listLow;
    private List<LimbSmoke> listMid;
    private List<LimbSmoke> listHigh;

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
        listLow = new List<LimbSmoke>();
        listMid = new List<LimbSmoke>();
        listHigh = new List<LimbSmoke>();
        
        LimbSmoke[] preListLow = lowParent.GetComponentsInChildren<LimbSmoke>();
        LimbSmoke[] preListMid = midParent.GetComponentsInChildren<LimbSmoke>();
        LimbSmoke[] preListHigh = highParent.GetComponentsInChildren<LimbSmoke>();
        
        int preCountLow = preListLow.Length;
        int preCountMid = preListMid.Length;
        int preCountHigh = preListHigh.Length;
        
        for (int i = 0; i < preCountLow; i++)
        {
            preListLow[i].poolParent = lowParent;
            preListLow[i].level = 3;
            listLow.Add(preListLow[i]);
            preListLow[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < preCountMid; i++)
        {
            preListMid[i].poolParent = midParent;
            preListMid[i].level = 2;
            listMid.Add(preListMid[i]);
            preListMid[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < preCountHigh; i++)
        {
            preListHigh[i].poolParent = highParent;
            preListHigh[i].level = 1;
            listHigh.Add(preListHigh[i]);
            preListHigh[i].gameObject.SetActive(false);
        }
        
        if (poolCount > preCountLow)
        {
            for (int i = 0; i < poolCount - preCountLow; i++)
            {
                GameObject go = Instantiate(prefabLow, lowParent).gameObject;
                var s = go.GetComponent<LimbSmoke>();
                s.poolParent = lowParent;
                s.level = 3;
                listLow.Add(s);
                go.SetActive(false);
            }
        }
        if (poolCount > preCountMid)
        {
            for (int i = 0; i < poolCount - preCountMid; i++)
            {
                GameObject go = Instantiate(prefabMid, midParent).gameObject;
                var s = go.GetComponent<LimbSmoke>();
                s.poolParent = midParent;
                s.level = 2;
                listMid.Add(s);
                go.SetActive(false);
            }
        }
        if (poolCount > preCountHigh)
        {
            for (int i = 0; i < poolCount - preCountHigh; i++)
            {
                GameObject go = Instantiate(prefabHigh, highParent).gameObject;
                var s = go.GetComponent<LimbSmoke>();
                s.poolParent = highParent;
                s.level = 1;
                listHigh.Add(s);
                go.SetActive(false);
            }
        }
    }

    public LimbSmoke Spawn(Vector3 pos, Vector3 forward, int damageLevel)
    {
        if (damageLevel == 0)
            return null;

        LimbSmoke d = null;
        if (damageLevel == 1)
        {
            d = listHigh.FirstOrDefault(x => !x.gameObject.activeSelf);
            listHigh.Remove(d);
        } else if (damageLevel == 2)
        {
            d = listMid.FirstOrDefault(x => !x.gameObject.activeSelf);
            listMid.Remove(d);
        } else if (damageLevel == 3)
        {
            d = listLow.FirstOrDefault(x => !x.gameObject.activeSelf);
            listLow.Remove(d);
        }
        if (d != null)
        {
            d.gameObject.SetActive(true);
            d.transform.position = pos;
            d.transform.forward = forward;
        }

        if (listHigh.Count == 0 || listMid.Count == 0 || listLow.Count == 0)
        {
            Init();
        }
        
        return d;
    } 
}
