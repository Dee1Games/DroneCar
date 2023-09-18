using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotShield : MonoBehaviour
{
    private Animator _animator;
    
    private static readonly int Pack = Animator.StringToHash("pack");
    private static readonly int PackH = Animator.StringToHash("pack_H");
    
    

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetPack(bool value)
    {
        _animator.SetBool(Pack, value);
    }

    public void SetPackHalfway(bool value)
    {
        _animator.SetBool(PackH, value);
    }
}
