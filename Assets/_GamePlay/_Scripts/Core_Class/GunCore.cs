using DG.Tweening;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using UnityEngine;

public class GunCore : MonoBehaviour
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
        if (TryGetComponent(out caster))
        {
            mainSensor = caster.raySensors[0];
            if (onCastParticle)
            {
                caster.onCast.AddListener(() =>
                {
                    Instantiate(onCastParticle, mainSensor.transform.position, mainSensor.transform.rotation);
                });
            }
            
            caster.trackTarget = CarCore._.transform;
        }
    }

    protected void Stop() => sequence.Kill();

    public void Activate()
    {
        if (caster)
        {
            // UI_Core._?.track.DoAlert(activeDelay, alertColor);
            DOVirtual.DelayedCall(activeDelay, () => caster.enabled = true);
        }
    }

    public void Deactivate()
    {
        if (caster)
        {
            // UI_Core._?.track.ResetAlert();
            caster.enabled = false;
        }
    }
}
