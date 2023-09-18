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
    
    public SightDetector sightDetector;
    public LookAtIK lookAtIK;

    protected Animator animator;
    protected NavMeshAgent agent;

    [Title("Animation")]
    public bool allowTurning;
    public bool allowSitting;

    [Title("IKs")] public AimIK[] aimIks;
    [Range(0, 1)]
    [SerializeField] private float handsUp;
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
    private static readonly int CarHeight = Animator.StringToHash("carHeight");

    private const string Sync = "Sync";
    
    private float signedAngle, absAngle;
    protected virtual void Update()
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
        lookAtIK.enabled = phase;
    }

    private float IKDelay = 1f;
    private float weight = 0;

    protected void SetIKsTarget(Transform _target)
    {
        Debug.Log($"Set Target to: {_target}");
        lookAtIK.solver.target = _target;
        foreach (var aimIk in aimIks)
        {
            aimIk.solver.target = _target;
        }
    }

    private Tween IkTween;
    protected virtual void OnPlayerFound(CarCore _core)
    {
        Debug.Log($"<color=#83FF5F>{_core}</color> founded.");
        UI_Core._?.track.Begin();
        SetIKsTarget(_core.transform);
        IkTween.SafeKill();
        IkTween = DOVirtual.Float(weight, 1, IKDelay, f =>
        {
            weight = f;
            lookAtIK.solver.IKPositionWeight = weight;
            foreach (var aimIk in aimIks)
            {
                aimIk.solver.IKPositionWeight = weight;
            }
        });
    }
    protected virtual void OnPlayerLost(CarCore _core)
    {
        Debug.Log($"<color=#83FF5F>{_core}</color> Lost.");
        UI_Core._?.track.End();
        IkTween.SafeKill();
        IkTween = DOVirtual.Float(weight, 0, IKDelay, f =>
        {
            weight = f;
            lookAtIK.solver.IKPositionWeight = weight;
            foreach (var aimIk in aimIks)
            {
                aimIk.solver.IKPositionWeight = weight;
            }
        }).OnComplete(() =>  SetIKsTarget(null));
    }
}
