using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class City : MonoBehaviour
{
    [SerializeField] private List<Material> cityMats;
    [SerializeField] private List<Material> windowMaterials;
    [SerializeField] private Material buildingMat;
    [SerializeField] private CityColorScriptableObject cityColor;

    [Button]
    public void MakeGreyscale()
    {
        foreach (var cityMat in cityMats)
        {
            cityMat.EnableKeyword("_ISGREY");
        }
        
        foreach (var windowsMat in windowMaterials)
        {
            windowsMat.EnableKeyword("_ISGREY");
        }
        
        buildingMat.EnableKeyword("_ISGREY");
        
    }
    
    [Button]
    public void MakeNormal()
    {
        foreach (var cityMat in cityMats)
        {
            cityMat.DisableKeyword("_ISGREY");
        }
        
        foreach (var windowsMat in windowMaterials)
        {
            windowsMat.DisableKeyword("_ISGREY");
        }
        
        buildingMat.DisableKeyword("_ISGREY");

    }

    [Button]
    public void ChangeBuildingColors()
    {
        foreach (var windowMat in windowMaterials)
        {
            windowMat.SetColor("_WindowColor01", cityColor.Color1);
            windowMat.SetColor("_WindowColor02", cityColor.Color2);
            windowMat.SetColor("_WindowColorEmissive", cityColor.EmissiveColor);
        }
        
        foreach (var cityMat in cityMats)
        {
            cityMat.SetColor("_WindowColor01", cityColor.Color1);
        }
        buildingMat.SetColor("_BaseColor", cityColor.baseColor);
        
    }

}
