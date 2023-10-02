using System.Collections.Generic;
using System.Linq;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

public class Giant_Core : MonoBehaviour, IHitable
{
    public Monster monster;
    protected AI_Core aiCore;
    protected Animator animator;
    [HideInInspector] public FullBodyBipedIK fullBodyBipedIK;

    [Title("Options", titleAlignment: TitleAlignments.Centered)]
    [Range(0, 1)]
    public float armor;


    [Title("UI")]
    public Sprite giantIcon;
    
    [FoldoutGroup("Ragdoll Setup")]
    public bool hasRagdoll;
    [FoldoutGroup("Ragdoll Setup")]
    public List<Rigidbody> ragdollParts;
    [FoldoutGroup("Ragdoll Setup")]
    public Limb[] limbs;
    [FoldoutGroup("Ragdoll Setup")]
    public float mass = 15;
    [FoldoutGroup("Ragdoll Setup")]
    public float drag = 3;
    [FoldoutGroup("Ragdoll Setup")]
    public float angularDrag = 3;

    private void Awake()
    {
        aiCore = GetComponent<AI_Core>();
    }

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
        animator = GetComponentInChildren<Animator>();
        fullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
        
        aiCore = GetComponent<AI_Core>();
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
    
    [FoldoutGroup("Ragdoll Setup")]
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
    [FoldoutGroup("Ragdoll Setup")]
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
    [FoldoutGroup("Ragdoll Setup")]
    private void FindLimbs()
    {
        limbs = GetComponentsInChildren<Limb>();
    }
    
    [FoldoutGroup("Ragdoll Setup")]
    public ParticleSystem particleSystem;
    
    [Button("Set Particles")]
    [FoldoutGroup("Ragdoll Setup")]
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
