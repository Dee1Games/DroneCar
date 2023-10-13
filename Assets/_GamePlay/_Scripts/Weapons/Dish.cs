using DG.Tweening;
using RaycastPro.RaySensors;
using UnityEngine;

public class Dish : Packable
{
    public TargetRay targetRay;
    public Transform target;

    public ParticleSystem effect;

    private Sequence _sequence;
    public void Activate()
    {
        _sequence = DOTween.Sequence();
        _sequence.AppendCallback(Unpack);
        _sequence.AppendInterval(2f);
        _sequence.AppendCallback(() =>
        {
            targetRay.target = CarCore._.transform;
            targetRay.linerEndPosition = 0;
        });
        _sequence.Append(DOVirtual.Float(targetRay.linerEndPosition, 1, 1f, f =>
        {
            targetRay.linerEndPosition = f;
        }));
        _sequence.AppendInterval(2f);
        _sequence.Append(DOVirtual.Float(targetRay.linerEndPosition, 0, 1f, f =>
        {
            targetRay.linerEndPosition = f;
        }));
        _sequence.AppendCallback(Pack);
        _sequence.Play();
    }
    
    
}
