using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LimbSmokePool : MonoBehaviour
{
    public static LimbSmokePool Instance;

    [SerializeField] private LimbSmoke prefabLow;
    [SerializeField] private LimbSmoke prefabMid;
    [SerializeField] private LimbSmoke prefabHigh;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public LimbSmoke Spawn(Vector3 pos, Vector3 forward, Transform parent, int damageLevel)
    {
        if (damageLevel == 0)
            return null;

        LimbSmoke d = null;
        if (damageLevel == 1)
        {
            GameObject go = Instantiate(prefabHigh).gameObject;
            d = go.GetComponent<LimbSmoke>();
            d.level = 1;
            
        } else if (damageLevel == 2)
        {
            GameObject go = Instantiate(prefabMid).gameObject;
            d = go.GetComponent<LimbSmoke>();
            d.level = 2;
        } else if (damageLevel == 3)
        {
            GameObject go = Instantiate(prefabLow).gameObject;
            d = go.GetComponent<LimbSmoke>();
            d.level = 3;
        }
        if (d != null)
        {
            d.transform.position = pos;
            d.transform.forward = forward;
            d.transform.parent = parent;
        }
        
        return d;
    } 
}
