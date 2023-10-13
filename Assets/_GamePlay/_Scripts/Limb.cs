using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public enum LimbType
{
    Head,
    LeftHand,
    RightHand,
    LeftFoot,
    RightFoot,
}
public class Limb : MonoBehaviour, IHitable
{
    private List<Limb> childLimbs;

    private Rigidbody _rigidbody;
    private Collider _collider;
    private Joint _joint;
    
    [HideInInspector] public Giant_Core giantCore;
    
    [Header("General")]
    public LimbType limbType;

    [Range(0, 1)]
    public float armor;
    public float health = 100f;
    public float maxHealth = 100f;

    public float Health
    {
        get => health;
        set
        {
            health = value;
            health = Mathf.Clamp(health, 0f, maxHealth);

            if (health <= 0)
            {
                Dismember();
            }
        }
    }
    /// <summary>
    /// For parts like foot and head
    /// </summary>
    public bool giantFinisher;
    
    /// <summary>
    /// This will stop run dismember Method
    /// </summary>
    public bool unbreakable;

    [Header("Art")]
    public ParticleSystem particle;
    public GameObject member;
    public Transform forceToDeScale;

    void Start()
    {
        _joint = GetComponent<Joint>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        childLimbs = GetComponentsInChildren<Limb>().ToList();
    }
    public void TakeDamage(float amount)
    {
        amount *= (1 - armor);
        Health -= amount;
        giantCore.TakeDamage(amount);
    }
    

    public void GetHit()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);

        if (_joint) Destroy(_joint);
        if (_rigidbody) Destroy(_rigidbody);
        if (_collider) Destroy(_collider);
        
        Destroy(this);
    }
    
    [Button("Dismember")]
    public void Dismember()
    {
        if (unbreakable) return;

        if (forceToDeScale)
        {
            forceToDeScale.localScale = Vector3.zero;
        }
        
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
            var gameObject = Instantiate(member, transform.position, transform.rotation, null);
            gameObject.transform.localScale = giantCore.transform.localScale;
        }
        
        if (giantFinisher)
        {
            // Ultra Damage
            giantCore.SetHealth(0);
        }
    }

    public void OnHit(CarCore core, float damage) => TakeDamage(damage);
}
