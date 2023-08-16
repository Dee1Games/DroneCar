using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Giant_AI : AI_Core
{
    private Animator animator;
    public NavMeshAgent agent;

    public Transform target;
    
    // Performance
    private static readonly int Speed = Animator.StringToHash("speed");

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
    }

    private void Update()
    {
        animator.SetFloat(Speed, agent.velocity.magnitude);
        agent.destination = target.position;
    }
}
