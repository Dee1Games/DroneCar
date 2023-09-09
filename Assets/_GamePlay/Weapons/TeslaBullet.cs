using System.Collections;
using System.Collections.Generic;
using RaycastPro.Detectors;
using UnityEngine;

public class TeslaBullet : MonoBehaviour
{
    public RangeDetector explodeArea;
    public float damage = 70f;

    public Transform effects;
    void Start()
    {
        if (!explodeArea)
        {
            explodeArea = GetComponentInChildren<RangeDetector>();
        }
    }

    private float _distanceValue;
    public void Activate()
    {
        if (explodeArea.Cast())
        {
            foreach (var collider in explodeArea.DetectedColliders)
            {
                if (collider.TryGetComponent(out CarCore CarCore))
                {
                    _distanceValue = 1 - Mathf.Clamp01(Vector3.Distance(transform.position, collider.transform.position) / explodeArea.Radius);
                    CarCore.TakeDamage(_distanceValue * damage);
                }

                
            }
        }
        
        if (effects)
        {
            Instantiate(effects, transform.position, transform.rotation);
        }
    }
}
