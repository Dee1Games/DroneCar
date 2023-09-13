using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using UnityEngine;

public class IceGun : GunCore
{
    public float alertTime = 1f;
    public float trackTime = 6f;

    public void SetTarget(Transform target) => caster.trackTarget = target;
    public override void Activate()
    {
        caster.enabled = true;
        UI_Core._.track.DoAlert(alertTime, alertColor);
    }

    public override void Deactivate()
    {
        caster.enabled = false;
        UI_Core._.track.ResetAlert();
    }
}
