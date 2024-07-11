using System;
using System.Collections;
using System.Collections.Generic;
using Obi;
using UnityEngine;

public class ClothGate : MonoBehaviour
{
    [SerializeField] private ObiTearableCloth cloth;
    [SerializeField] private bool knockback;

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
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerVehicle p = other.GetComponentInParent<PlayerVehicle>();
        if (knockback && p != null)
        {
            p.JumpBack();
        }
    }
}
