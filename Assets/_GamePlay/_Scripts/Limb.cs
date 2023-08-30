using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Limb : MonoBehaviour
{
    private List<Limb> childLimbs;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private Joint _joint;
    [HideInInspector]
    public Giant_Core giantCore;

    [Header("General")]
    public float health = 100f;
    public float maxHealth = 100f;

    /// <summary>
    /// For parts like foot and head
    /// </summary>
    public bool ragdollActivator;
    
    /// <summary>
    /// This will stop run dismember Method
    /// </summary>
    public bool unbreakable;

    [Header("Art")]
    public ParticleSystem particle;
    public GameObject member;


    void Start()
    {
        _joint = GetComponent<Joint>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        childLimbs = GetComponentsInChildren<Limb>().ToList();
    }

#if UNITY_EDITOR
    [Header("Debug")] public KeyCode pressToDismember;
    private void Update()
    {
        if (pressToDismember != null)
        {
            if (Input.GetKeyDown(pressToDismember))
            {
                Dismember();
            }
        }
    }
#endif


    public void TakeDamage(float amount)
    {
        health -= amount;

        amount = Mathf.Clamp(amount, 0f, maxHealth);
        if (amount <= 0)
        {
            Dismember();
        }
    }

    public void GetHit()
    {
        transform.localScale = Vector3.zero;

        if (_joint) Destroy(_joint);
        if (_rigidbody) Destroy(_rigidbody);
        if (_collider) Destroy(_collider);
        
        Destroy(this);
    }
    public void Dismember()
    {
        if (unbreakable) return;
        
        foreach (var childLimb in childLimbs)
        {
            if (childLimb)
            {
                childLimb.GetHit();
            }
        }

        if (particle) Instantiate(particle, transform.position, transform.rotation, transform.root);
        if (member)
        {
            Instantiate(member, transform.position, transform.rotation, transform.root);
        }
        
        if (ragdollActivator)
        {
            giantCore.RagdollSetActive(true);
        }
    }
}
