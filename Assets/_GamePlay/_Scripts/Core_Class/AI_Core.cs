using DG.Tweening;
using RaycastPro.Detectors;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Implementation basic AI features of all Giants
/// </summary>
public class AI_Core : MonoBehaviour
{
    protected Giant_Core myCore;

    [Range(0, 1)]
    public float hardiness = 0f;
    
    protected SightDetector sightDetector;
    protected LookAtIK lookAtIK;

    protected Animator animator;
    protected NavMeshAgent agent;

    [Title("Animation")]
    public bool allowTurning = true;
    public bool allowSitting;

    [Title("IKs")] public AimIK[] aimIks;
    
    [Title("Updates")]
    public IK[] IKs;
    protected virtual void Update()
    {
        foreach (var ik in IKs)
        {
            if (!ik.enabled)
            {
                ik.GetIKSolver().Update();
            }
        }
    }

    [Range(0, 1)]
    [SerializeField] private float handsUp;

    public bool refreshAimIks;
    public float HandsUp
    {
        get => handsUp;
        set
        {
            handsUp = value;
            animator.SetLayerWeight(1, value);
        }
    }
    
    
    public NavMeshAgent Agent => agent;
    
    protected CarCore carCore;
    
    protected void Awake()
    {
        myCore = GetComponent<Giant_Core>();
        lookAtIK = GetComponentInChildren<LookAtIK>();
        sightDetector = GetComponentInChildren<SightDetector>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();

        sightDetector?.onNewCollider.AddListener(col =>
        {
            if (col.TryGetComponent(out carCore))
            {
                OnPlayerFound(carCore);
            }
        });
        sightDetector?.onLostCollider.AddListener(col =>
        {
            OnPlayerLost(carCore);
            carCore = null;
        });
    }

    private Vector3 _direction;
    
    private static readonly int TurnAngle = Animator.StringToHash("turnAngle");
    private static readonly int Mirror = Animator.StringToHash("mirror");
    private static readonly int CarHeight = Animator.StringToHash("carHeight");

    private const string Sync = "Sync";
    
    private float signedAngle, absAngle;
    protected virtual void FixedUpdate()
    {
        if (allowTurning)
        {
            if (carCore)
            {
                _direction = CarCore._.transform.position - transform.position;
                _direction.y = 0;
                signedAngle = Vector3.SignedAngle(_direction, transform.forward, transform.up);
                absAngle = Mathf.Abs(signedAngle);
                if (absAngle > 25)
                {
                    animator.SetBool(Mirror, signedAngle > 0);
                    animator.SetFloat(TurnAngle, absAngle);
                }
                else
                {
                    animator.SetFloat(TurnAngle, 0);
                }
            }
            else
            {
                animator.SetFloat(TurnAngle, 0);
            }
        }

        if (allowSitting)
        {
            if (CarCore._)
            {
                animator.SetFloat(CarHeight, CarCore._.transform.position.y);
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
        sightDetector.enabled = phase;
        if (lookAtIK) lookAtIK.enabled = phase;
    }

    private float IKDelay = 1f;
    private float weight = 0;

    protected void SetIKsTarget(Transform _target)
    {
        Debug.Log($"Set Target to: {_target}");
        if (lookAtIK)
        {
            lookAtIK.solver.target = _target;
        }
        foreach (var aimIk in aimIks)
        {
            aimIk.solver.target = _target;
        }
    }

    private Tween IkTween;

    protected virtual void OnPlayerFound(CarCore _core)
    {
        Debug.Log($"<color=#83FF5F>{_core}</color> founded.");

        if (UI_Core._) UI_Core._.track.Begin();
        
        SetIKsTarget(_core.transform);
        IkTween.SafeKill();
        IkTween = DOVirtual.Float(weight, 1, IKDelay, f =>
        {
            weight = f;
            if (lookAtIK)
            {
             lookAtIK.solver.IKPositionWeight = weight;
            }
            foreach (var aimIk in aimIks)
            {
                aimIk.solver.IKPositionWeight = weight;
            }
        });
    }
    protected virtual void OnPlayerLost(CarCore _core)
    {
        Debug.Log($"<color=#83FF5F>{_core}</color> Lost.");

        if (UI_Core._) UI_Core._.track.End();

        IkTween.SafeKill();
        IkTween = DOVirtual.Float(weight, 0, IKDelay, f =>
        {
            weight = f;
            if (lookAtIK)
            {
                lookAtIK.solver.IKPositionWeight = weight;
            }
            foreach (var aimIk in aimIks)
            {
                aimIk.solver.IKPositionWeight = weight;
            }
        }).OnComplete(() =>  SetIKsTarget(null));
    }
}
