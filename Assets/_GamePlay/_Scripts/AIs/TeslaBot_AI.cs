using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using RootMotion;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class TeslaBot_AI : AI_Core
{
    [Title("Weapons")]
    public AdvanceCaster TeslaGun;
    public Shield shield;
    
    [Title("General")]
    public float teslaReadyTime = 3f;
    public Tween teslaTween;

    public void Start()
    {
        if (shield)
        {
            shield.Activate(myCore.fullBodyBipedIK.references.head, myCore);
        }
    }

    private bool isAlerting;
    public void TurnTesla(bool active)
    {
        if (active)
        {
            if (isAlerting) return;
            isAlerting = true;
            UI_Core._.track.DoAlert(teslaReadyTime, Color.red);
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
            UI_Core._.track.ResetAlert();
            isAlerting = false;
            if (teslaTween != null && teslaTween.IsPlaying())
            {
                teslaTween.Kill();
            }
        }
    }

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        TurnTesla(true);
    }
    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        TurnTesla(false);
    }
}
