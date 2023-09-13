using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explodable : MonoBehaviour
{
    [SerializeField] private float fadeDelay = 10f;
    
    private Rigidbody[] rigidbodies;
    
    private bool exploded = false;

    private void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    private void Start()
    {
        foreach (Rigidbody rigid in rigidbodies)
        {
            rigid.isKinematic = true;
            rigid.useGravity = false;
        }
        exploded = false;
    }

    public void Explode(Vector3 pos, float radius, float force)
    {
        if (exploded)
            return;
        
        foreach (Rigidbody rigid in rigidbodies)
        {
            rigid.isKinematic = false;
            rigid.useGravity = true;
            rigid.AddExplosionForce(force, pos, radius, 0.3f, ForceMode.VelocityChange);
        }

        exploded = true;
        
        Destroy(gameObject, fadeDelay);
    }
}
