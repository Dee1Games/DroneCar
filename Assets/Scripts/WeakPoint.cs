using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour
{
    private Explodable explodable;
    
    private void Awake()
    {
        explodable = GetComponentInChildren<Explodable>();
    }

    public void Hit()
    {
        explodable.transform.parent = null;
        explodable.Explode(transform.position, 10f, 50f);
    }
}
