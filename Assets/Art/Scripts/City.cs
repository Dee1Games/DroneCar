using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class City : MonoBehaviour
{
    [SerializeField] private List<Material> cityMats;

    [Button]
    public void MakeGreyscale()
    {
        foreach (var cityMat in cityMats)
        {
            cityMat.EnableKeyword("_ISGREY");
        }
    }
    
    [Button]
    public void MakeNormal()
    {
        foreach (var cityMat in cityMats)
        {
            cityMat.DisableKeyword("_ISGREY");
        }
    }

}
