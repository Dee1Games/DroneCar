using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : Packable
{
    [SerializeField] private float sawSpeed;
    private static readonly int CSaw = Animator.StringToHash("saw");

    public float SawSpeed
    {
        get => sawSpeed;
        set
        {
            sawSpeed = value;
            _animator.SetFloat(CSaw, value);
        }
    }
}
