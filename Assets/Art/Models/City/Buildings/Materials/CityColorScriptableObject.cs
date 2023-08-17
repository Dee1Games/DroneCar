using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "City Properties", order = 2, fileName = "City Colors")]
public class CityColorScriptableObject : ScriptableObject
{
    public Color baseColor;

    public Color Color1;
    public Color Color2;
    public Color EmissiveColor;
}