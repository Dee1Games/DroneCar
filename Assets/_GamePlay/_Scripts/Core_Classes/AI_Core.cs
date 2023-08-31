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
            col.TryGetComponent(out playerVehicle);
            OnPlayerFound(playerVehicle);
            Debug.Log($"<color=#83FF5F>{playerVehicle}</color> founded.");
        });
        sightDetector.onLostCollider.AddListener(col =>
        {
            Debug.Log($"<color=#83FF5F>{playerVehicle}</color> Lost.");
            OnPlayerLost(playerVehicle);
            playerVehicle = null;
        });
    }

    public virtual void Active(bool phase)
    {
        animator.enabled = phase;
        sightDetector.enabled = phase;
        lookAtIK.enabled = phase;
    }
    protected void DoLook(float value, float duration)
    {
        DOVirtual.Float(lookAtIK.solver.GetIKPositionWeight()
            ,value, duration, f => lookAtIK.solver.SetLookAtWeight(f));
    }
    private float lookRateDelay = 1f;
    protected virtual void OnPlayerFound(PlayerVehicle vehicle)
    {
        lookAtIK.solver.target = vehicle.transform;
        DoLook(1, lookRateDelay);
    }
    protected virtual void OnPlayerLost(PlayerVehicle vehicle)
    {
        lookAtIK.solver.target = null;
        DoLook(0, lookRateDelay);
    }
}
