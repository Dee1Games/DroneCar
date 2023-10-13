using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using Sirenix.OdinInspector;
using UnityEngine;

public class MiniGun : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private AdvanceCaster _advanceCaster;

    public float maxSpeed = 1.5f;
    public float tweenDuration = 1f;
    
    private static readonly int Speed = Animator.StringToHash("speed");

    void Start()
    {
        _animator = GetComponent<Animator>();
        _advanceCaster = GetComponent<AdvanceCaster>();
    }

    private Tween _tween;
    
    [HorizontalGroup]
    [Button("Activate")]
    [GUIColor("#2EFF77")]
    public void Activate()
    {
        _tween.SafeKill();
        _tween = DOVirtual.Float(_animator.GetFloat(Speed), maxSpeed, tweenDuration, f =>
        {
            _animator.SetFloat(Speed, f);
        });
        _advanceCaster.enabled = true;
    }
    
    [HorizontalGroup]
    [GUIColor("#FF4643")]
    [Button("Deactivate")]
    public void Deactivate()
    {
        _tween.SafeKill();
        _tween = DOVirtual.Float(_animator.GetFloat(Speed), 0, tweenDuration, f =>
        {
            _animator.SetFloat(Speed, f);
        });
        _advanceCaster.enabled = false;
    }
}
