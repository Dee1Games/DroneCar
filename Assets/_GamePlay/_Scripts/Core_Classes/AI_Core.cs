using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Implementation basic AI features of all Giants
/// </summary>
public class AI_Core : MonoBehaviour
{
    [Range(0, 1)]
    public float hardiness = 0f;
    
    protected Animator animator;
    protected NavMeshAgent agent;

    public NavMeshAgent Agent => agent;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
    }
}
