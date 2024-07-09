using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class ClothGate : MonoBehaviour
{
    [SerializeField] private ObiTearableCloth cloth;

    public void Hit(Vector3 vector)
    {
        float minZ = cloth.transform.position.z;
        if (vector.z < minZ)
        {
            vector.z = minZ + 0.1f;
        }

        vector *= 0.2f;
        // cloth.AddForce(vector, ForceMode.Impulse);
    }
}
