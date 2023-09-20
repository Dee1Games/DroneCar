using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using UnityEngine;

public class IceGun : GunCore
{
    public float delayTime = 1f;
    public void SetTarget(Transform target) => caster.trackTarget = target;
}
