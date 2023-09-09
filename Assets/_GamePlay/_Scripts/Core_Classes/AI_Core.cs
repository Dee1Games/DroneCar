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
    private Giant_Core _giantCore;


    
    [Range(0, 1)]
    public float hardiness = 0f;
    
    public SightDetector sightDetector;
    public LookAtIK lookAtIK;

    protected Animator animator;
    protected NavMeshAgent agent;

    [Header("Animation")]
    public bool allowTurning;
    public NavMeshAgent Agent => agent;
    
    protected CarCore carCore;

    protected void Awake()
    {
        _giantCore = GetComponent<Giant_Core>();
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
            if (col.TryGetComponent(out carCore))
            {
                OnPlayerFound(carCore);
            }
        });
        sightDetector.onLostCollider.AddListener(col =>
        {
            OnPlayerLost(carCore);
            carCore = null;
        });
    }

    private Vector3 _direction;
    
    private static readonly int TurnAngle = Animator.StringToHash("turnAngle");
    private static readonly int Mirror = Animator.StringToHash("mirror");

    private const string Sync = "Sync";
    
    private float signedAngle, absAngle;
    public virtual void Update()
    {
        if (allowTurning && !animator.GetCurrentAnimatorStateInfo(0).IsTag(Sync))
        {
            if (carCore)
            {
                _direction = carCore.transform.position - transform.position;
                _direction.y = 0;
                signedAngle = Vector3.SignedAngle(_direction, transform.forward, transform.up);
                absAngle = Mathf.Abs(signedAngle);
                if (absAngle > 10)
                {
                    animator.SetBool(Mirror, signedAngle > 0);
                    animator.SetFloat(TurnAngle, absAngle);
                }
            }
            else
            {
                animator.SetFloat(TurnAngle, 0);
            }
        }
    }

    public void OnEnd(CarCore vehicle)
    {
        OnPlayerLost(vehicle);
    }
    /// <summary>
    /// This will use when die or restore function active to disable IKs and animator
    /// </summary>
    /// <param name="phase"></param>
    public virtual void Active(bool phase)
    {
        animator.enabled = phase;
        sightDetector.enabled = phase;
        lookAtIK.enabled = phase;
    }
    private float lookRateDelay = 1f;



    protected virtual void OnPlayerFound(CarCore vehicle)
    {
        Debug.Log($"<color=#83FF5F>{vehicle}</color> founded.");

        lookAtIK.solver.target = vehicle.transform;
        DOVirtual.Float(lookAtIK.solver.IKPositionWeight
            ,1, lookRateDelay, f => lookAtIK.solver.IKPositionWeight = f);
    }
    protected virtual void OnPlayerLost(CarCore vehicle)
    {
        Debug.Log($"<color=#83FF5F>{vehicle}</color> Lost.");
        DOVirtual.Float(lookAtIK.solver.IKPositionWeight
            ,0, lookRateDelay, f => lookAtIK.solver.IKPositionWeight = f).OnComplete(() =>
        {
            lookAtIK.solver.target = null;
        });
    }
}
