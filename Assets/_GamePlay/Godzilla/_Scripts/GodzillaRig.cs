using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.AI;

public class GodzillaRig : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    [Header("IKs")]
    public LookAtIK lookAtIK;
    public CCDIK leftHand;
    public CCDIK rightHand;
    public LimbIK leftFoot;
    public LimbIK rightFoot;
    public FABRIK tail;

    public Transform target;
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

    }
    void Update()
    {
        _navMeshAgent.destination = target.position;
    }
}
