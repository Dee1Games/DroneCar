using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

public class Giant_Base : MonoBehaviour
{
    public Animator animator;
    public FullBodyBipedIK fullBodyBipedIK;
    
    public List<Rigidbody> ragdollParts;
    public Giant_AI giantAI;

    public Limb[] limbs;

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

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        fullBodyBipedIK = GetComponentInChildren<FullBodyBipedIK>();
        // Get All Ragdolls via ignore self;
        ragdollParts = GetComponentsInChildren<Rigidbody>().ToList();

        limbs = GetComponentsInChildren<Limb>();
        foreach (var limb in limbs)
        {
            limb.giantBase = this;
        }
    }

    public void RagdollSetActive(bool phase)
    {
        animator.enabled = !phase;
        if (giantAI) giantAI.agent.enabled = !phase;

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
