using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RaycastPro.Casters;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class Giant_Core : MonoBehaviour
{
    public Animator animator;
    public FullBodyBipedIK fullBodyBipedIK;
    
    public AI_Core aiCore;

    public List<Rigidbody> ragdollParts;
    public Limb[] limbs;
    [Header("Ragdoll Setup")]
    public float drag = 3;
    public float angularDrag = 3;
    
    private void Start()
    {
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
    }

    [Button("Set Ragdoll")]
    private void SetRagdoll()
    {
        ragdollParts = GetComponentsInChildren<Rigidbody>().ToList();
        foreach (var ragdollPart in ragdollParts)
        {
            ragdollPart.drag = drag;
            ragdollPart.angularDrag = angularDrag;
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
    
    public void RagdollSetActive(bool phase)
    {
        aiCore.Active(!phase);
        if (fullBodyBipedIK) fullBodyBipedIK.enabled = !phase;
        foreach (var ragdollPart in ragdollParts)
        {
            if (ragdollPart)
            {
                ragdollPart.isKinematic = !phase;
                ragdollPart.useGravity = phase;
            }
        }
    }
}
