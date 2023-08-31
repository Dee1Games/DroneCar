using RaycastPro.Detectors;
using UnityEngine;

public class Giant_AI : AI_Core
{

    public Transform target;
    
    // Performance
    private static readonly int Speed = Animator.StringToHash("speed");

    private void Update()
    {
        if (animator) animator.SetFloat(Speed, agent.velocity.magnitude);
        if (target) agent.destination = target.position;
    }
    
}
