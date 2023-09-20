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

    public Limb[] limbs;

    [Button("Find Limbs")]
    private void FindLimbs()
    {
        limbs = GetComponentsInChildren<Limb>();
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
