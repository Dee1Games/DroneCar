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
    private static readonly int m_Recoil = Animator.StringToHash("recoil");

    void Start()
    {
        _advanceCaster?.onRate.AddListener(() =>
        {
            animator.SetTrigger(m_Recoil);
        });
    }
}
