using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using UnityEngine;

public class GunCore : MonoBehaviour
{
    public AdvanceCaster caster;
    [SerializeField] protected Color alertColor = Color.red;
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
            caster.onCast.AddListener(() =>
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
    
    public void ShakeCamera()
    {
        var amp = Vector3.Distance(transform.position, CarCore._.transform.position);
        amp = Mathf.Clamp01((50 - amp) / 50f);
        if (amp > 0)
        {
            CameraController.Instance.ShakeCamera(.2f, amp);
        }
    }
}
