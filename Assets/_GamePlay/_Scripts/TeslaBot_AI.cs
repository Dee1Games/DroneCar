using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using RootMotion;
using RootMotion.FinalIK;
using Sirenix.Serialization;
using UnityEngine;

public class TeslaBot_AI : AI_Core
{
    [Header("Guns")]
    public AdvanceCaster TeslaGun;
    public Shield shield;
    
    public float teslaReadyTime = 3f;
    public Tween teslaTween;

    public void Start()
    {
        shield.Activate(myCore.fullBodyBipedIK.references.head, myCore);
    }

    private bool isAlerting;

    public void TurnTesla(bool active)
    {
        if (active)
        {
            if (isAlerting) return;
            isAlerting = true;
            UI_Core._.track.DoAlert(teslaReadyTime);
            teslaTween = DOVirtual.DelayedCall(teslaReadyTime, () =>
            {
                TeslaGun.enabled = true;
            });
            TeslaGun.trackTarget = carCore.transform;
        }
        else
        {
            UI_Core._.track.alertImage.fillAmount = 0;
            TeslaGun.enabled = false;
            TeslaGun.trackTarget = null;
            if (teslaTween != null && teslaTween.IsPlaying())
            {
                teslaTween.Kill();
                isAlerting = false;
            }
        }
    }

    protected override void OnPlayerFound(CarCore vehicle)
    {
        base.OnPlayerFound(vehicle);
        TurnTesla(true);
    }
    protected override void OnPlayerLost(CarCore vehicle)
    {
        base.OnPlayerLost(vehicle);
        TurnTesla(false);
    }
}
