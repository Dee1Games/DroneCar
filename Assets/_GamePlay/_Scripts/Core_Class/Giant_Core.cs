using System.Collections.Generic;
using System.Linq;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

public class Giant_Core : MonoBehaviour, IHitable
{
    public Monster monster;
    public Animator animator;
    
    public FullBodyBipedIK fullBodyBipedIK;

    
    [Title("Options", titleAlignment: TitleAlignments.Centered)]
    [Range(0, 1)]
    public float armor;
    
    [Title("AI")]
    public AI_Core aiCore;

    [Title("UI")]
    public Sprite giantIcon;



    [Title("Ragdoll Setup")] public bool hasRagdoll;
    public List<Rigidbody> ragdollParts;
    public Limb[] limbs;
    public float mass = 15;
    public float drag = 3;
    public float angularDrag = 3;
    
    public Transform GetRandomMember()
    {
        var random = Random.Range(0, 5);

        switch (random)
        {
            case 0: return fullBodyBipedIK.references.head;
            case 1: return fullBodyBipedIK.references.leftUpperArm;
            case 2: return fullBodyBipedIK.references.rightUpperArm;
            case 3: return fullBodyBipedIK.references.leftThigh;
            case 4: return fullBodyBipedIK.references.rightThigh;
        }
        return fullBodyBipedIK.references.head;
    }
    private void Start()
    {
        monster = GetComponentInParent<Monster>();
        aiCore = GetComponent<AI_Core>();
        animator = GetComponentInChildren<Animator>();
        fullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
        // Get All Ragdolls via ignore self;
        ragdollParts = GetComponentsInChildren<Rigidbody>().ToList();

        limbs = GetComponentsInChildren<Limb>();
        foreach (var limb in limbs)
        {
            limb.giantCore = this;
        }

        if (UI_Core._)
        {
            UI_Core._.giantIcon.sprite = giantIcon;
        }
    }
    
    [Button("Set Ragdoll")]
    private void SetRagdoll()
    {
        ragdollParts = GetComponentsInChildren<Rigidbody>().ToList();
        foreach (var ragdollPart in ragdollParts)
        {
            ragdollPart.mass = mass;
            ragdollPart.drag = drag;
            ragdollPart.angularDrag = angularDrag;
        }
    }

    [Button("Remove Mesh Colliders")]
    public void RemoveMeshColliders()
    {
        var children = GetComponentsInChildren<MeshCollider>();
        for (var index = 0; index < children.Length; index++)
        {
            var child = children[index];
            DestroyImmediate(child);
        }
    }
    [Button("Find Limbs")]
    private void FindLimbs()
    {
        limbs = GetComponentsInChildren<Limb>();
    }

    public ParticleSystem particleSystem;
    
    [Button("Set Particles")]
    public void SetParticles()
    {
        foreach (var limb in limbs)
        {
            limb.particle = particleSystem;
        }
    }

    private static readonly int Die = Animator.StringToHash("die");
    
    private bool isDead;

    public bool IsDead => isDead;
    
    /// <summary>
    /// False = die
    /// </summary>
    /// <param name="phase"></param>
    public void OnDie()
    {
        isDead = true;
        aiCore.Active(false);

        if (hasRagdoll)
        {
            animator.enabled = false;
            if (fullBodyBipedIK) fullBodyBipedIK.enabled = false;
            foreach (var ragdollPart in ragdollParts)
            {
                if (ragdollPart)
                {
                    ragdollPart.isKinematic = false;
                    ragdollPart.useGravity = true;
                }
            }
        }
        else
        {
            animator.SetLayerWeight(1, 0);
            animator.SetLayerWeight(2, 0);
            animator.SetTrigger(Die);
        }
    }
    public void TakeDamage(float damage)
    {
        monster.Health -= damage * (1-armor);
        if (monster.Health <= 0)
        {
            OnDie();
        }
    }
    public void SetHealth(float amount)
    {
        monster.Health = amount;
        if (amount <= 0)
        {
            OnDie();
        }
    }
    public void OnHit(CarCore core, float damage) => TakeDamage(damage);
}
