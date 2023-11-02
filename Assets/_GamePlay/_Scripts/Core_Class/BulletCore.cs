using System.Collections;
using System.Collections.Generic;
using RaycastPro.Detectors;
using UnityEngine;

public class BulletCore : MonoBehaviour
{
    public float explosionRange;

    public void Activate()
    {
        if (!CarCore._) return;

        if (Vector3.Distance(CarCore._.transform.position, transform.position) <= explosionRange)
        {
            CarCore._.ApplySlowMotion();
        }
    }
}
