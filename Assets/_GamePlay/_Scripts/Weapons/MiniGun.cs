using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MiniGun : GunCore
{
    private Animator _animator;

    public float maxSpeed = 1.5f;
    public float beginDuration = 1f;
    public float alertDuration = 2f;
    
    private static readonly int Speed = Animator.StringToHash("speed");

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private Tween _tween;
    
    [HorizontalGroup]
    [Button("Activate")]
    [GUIColor("#2EFF77")]
    public void Activate()
    {
        _tween.SafeKill();
        _tween = DOVirtual.Float(_animator.GetFloat(Speed), maxSpeed, beginDuration, f =>
        {
            _animator.SetFloat(Speed, f);
        }).OnComplete(() =>
        {
            UI_Core._.track.DoAlert(alertDuration, alertColor, onComplete: () => caster.enabled = true);
        });
    }
    
    [HorizontalGroup]
    [GUIColor("#FF4643")]
    [Button("Deactivate")]
    public void Deactivate()
    {
        _tween.SafeKill();
        _tween = DOVirtual.Float(_animator.GetFloat(Speed), 0, beginDuration, f =>
        {
            _animator.SetFloat(Speed, f);
        });
        caster.enabled = false;
    }
}
