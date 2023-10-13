using System.Collections;
using System.Collections.Generic;
using RaycastPro.Casters;
using UnityEngine;

/// <summary>
/// Force to play Recoil Animation on target Animator
/// </summary>
public class Recoil : MonoBehaviour
{
    public AdvanceCaster _advanceCaster;
    public Animator animator;
    private static readonly int RecoilR = Animator.StringToHash("recoil_R");
    private static readonly int RecoilL = Animator.StringToHash("recoil_L");

    void Start()
    {
        _advanceCaster = GetComponent<AdvanceCaster>();
        _advanceCaster.onCast.AddListener( () =>
        {
            if (_advanceCaster.rayIndex == 0)
            {
                animator.SetTrigger(RecoilL);
            }
            else
            {
                animator.SetTrigger(RecoilR);
            }
        });
    }
}
