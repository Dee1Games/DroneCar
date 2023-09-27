using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Packable : MonoBehaviour
{
    protected Animator _animator;
    private static readonly int CPack = Animator.StringToHash("pack");

    public bool IsPacked { protected set; get; }
    void Start()
    {
        _animator = GetComponent<Animator>();
        IsPacked = _animator.GetBool(CPack);
    }
    
    [HorizontalGroup()]
    [Button("Pack")]
    public void Pack() => SetPack(true);
    
    [HorizontalGroup()]
    [Button("Unpack")]
    public void Unpack() => SetPack(false);
    
    public void SetPack(bool value)
    {
        IsPacked = value;
        _animator.SetBool(CPack, value);
    }
}
