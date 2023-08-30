using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Giant_AI : AI_Core
{

    public Transform target;
    
    // Performance
    private static readonly int Speed = Animator.StringToHash("speed");

    protected override void Start()
    {
        base.Start();
        
    }

    private void Update()
    {
        animator.SetFloat(Speed, agent.velocity.magnitude);

        if (target) agent.destination = target.position;
    }
}
