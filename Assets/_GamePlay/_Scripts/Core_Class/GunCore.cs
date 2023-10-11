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
    public float activeDelay;

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

    public void Activate()
    {
        UI_Core._?.track.DoAlert(activeDelay, alertColor);
        DOVirtual.DelayedCall(activeDelay, () => caster.enabled = true);
    }

    public void Deactivate()
    {
        UI_Core._?.track.ResetAlert();
        caster.enabled = false;
    }
}
