using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using UnityEngine;

public abstract class GunCore : MonoBehaviour
{
    public AdvanceCaster caster;
    public Color alertColor = Color.red;
    public float alertDelay = 1f;

    public ParticleSystem onCastParticle;

    protected Sequence sequence;

    protected bool IsPlaying => sequence != null && sequence.IsPlaying();

    private RaySensor mainSensor;
    private void Start()
    {
        TryGetComponent(out caster);

        mainSensor = caster.raySensors[0];
        if (onCastParticle)
        {
            caster.onCast.AddListener(b =>
            {
                Instantiate(onCastParticle, mainSensor.transform.position, mainSensor.transform.rotation);
            });
        }
    }

    protected void Stop() => sequence.Kill();
    public abstract void Activate();
    public abstract void Deactivate();
}
