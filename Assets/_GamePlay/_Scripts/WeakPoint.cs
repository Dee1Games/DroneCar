using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

public class WeakPoint : MonoBehaviour, IHitable
{
    public Giant_Core myCore;
    
    [Title("General")]
    public float damagePlus;
    public float damageMultiplier;

    [Title("Art")]
    private Explodable explodable;
    public Limb attachedLimb;
    public ParticleSystem explodeEffect;
    private void Awake()
    {
        if (!myCore) myCore = GetComponentInParent<Giant_Core>();
        explodable = GetComponentInChildren<Explodable>();
    }

    public void FindCore()
    {
        myCore = GetComponentInParent<Giant_Core>();
    }

    public void OnHit(CarCore core, float damage = 10f)
    {
        myCore.TakeDamage(damage * damageMultiplier + damagePlus);
        if (explodable)
        {
            explodable.transform.SetParent(null);
            explodable.Explode(transform.position);
        }
        if (explodeEffect)
        {
            var effect = Instantiate(explodeEffect);
            effect.transform.position = CarCore._.transform.position;
            effect.transform.up = -CarCore._.transform.forward;
        }
        attachedLimb?.Dismember();
    }
}
