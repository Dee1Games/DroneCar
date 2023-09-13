using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class WeakPoint : MonoBehaviour
{
    private Explodable explodable;
    public Limb attachedLimb;
    public ParticleSystem explodeEffect;
    private void Awake()
    {
        explodable = GetComponentInChildren<Explodable>();
    }

    public void Hit()
    {
        explodable.transform.SetParent(null);
        explodable.Explode(transform.position, 10f, 50f);

        if (explodeEffect)
        {
            var effect = Instantiate(explodeEffect);
            effect.transform.position = CarCore._.transform.position;
            effect.transform.up = -CarCore._.transform.forward;
        }

        attachedLimb?.Dismember();
    }
}
