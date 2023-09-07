using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Detectors;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Implementation basic AI features of all Giants
/// </summary>
public abstract class AI_Core : MonoBehaviour
{
    [Range(0, 1)]
    public float hardiness = 0f;
    
    public SightDetector sightDetector;
    public LookAtIK lookAtIK;

    protected Animator animator;
    protected NavMeshAgent agent;

    public NavMeshAgent Agent => agent;
    
    protected PlayerVehicle playerVehicle;
    

    protected virtual void Start()
    {

        
        if (!animator)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (!agent)
        {
            agent = GetComponentInChildren<NavMeshAgent>();
        }

        if (!lookAtIK)
        {
            lookAtIK = GetComponentInChildren<LookAtIK>();
        }
        if (!sightDetector)
        {
            sightDetector = GetComponentInChildren<SightDetector>();
        }

        sightDetector.onNewCollider.AddListener(col =>
        {
            if (col.TryGetComponent(out playerVehicle))
            {
                OnPlayerFound(playerVehicle);
            }
        });
        sightDetector.onLostCollider.AddListener(col =>
        {
            OnPlayerLost(playerVehicle);
        });
    }

    public void OnEnd(PlayerVehicle vehicle)
    {
        OnPlayerLost(vehicle);
    }
    public virtual void Active(bool phase)
    {
        animator.enabled = phase;
        sightDetector.enabled = phase;
        lookAtIK.enabled = phase;
    }
    private float lookRateDelay = 1f;
    protected virtual void OnPlayerFound(PlayerVehicle vehicle)
    {
        Debug.Log($"<color=#83FF5F>{vehicle}</color> founded.");

        lookAtIK.solver.target = vehicle.transform;
        DOVirtual.Float(lookAtIK.solver.IKPositionWeight
            ,1, lookRateDelay, f => lookAtIK.solver.IKPositionWeight = f);
    }
    protected virtual void OnPlayerLost(PlayerVehicle vehicle)
    {
        Debug.Log($"<color=#83FF5F>{vehicle}</color> Lost.");
        DOVirtual.Float(lookAtIK.solver.IKPositionWeight
            ,0, lookRateDelay, f => lookAtIK.solver.IKPositionWeight = f).OnComplete(() =>
        {
            lookAtIK.solver.target = null;
        });
    }
}
