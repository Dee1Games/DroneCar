using System.Collections;
using System.Collections.Generic;
using RaycastPro.Casters;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    private AdvanceCaster _advanceCaster;
    public Animator animator;
    private static readonly int RecoilR = Animator.StringToHash("recoil_R");
    private static readonly int RecoilL = Animator.StringToHash("recoil_L");

    void Start()
    {
        _advanceCaster = GetComponent<AdvanceCaster>();
        _advanceCaster.onCast.AddListener(b =>
        {
            Debug.Log(_advanceCaster.currentIndex);
            if (_advanceCaster.currentIndex == 0)
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
