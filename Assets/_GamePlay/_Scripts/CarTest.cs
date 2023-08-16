using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTest : MonoBehaviour
{
    
    public void Damage()
    {
        GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
    }
}
