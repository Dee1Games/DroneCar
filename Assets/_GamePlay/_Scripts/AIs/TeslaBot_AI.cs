using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using RootMotion;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
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
        shield?.SetTarget(myCore.fullBodyBipedIK.references.head);
        StartCoroutine(ShieldRun());
    }

    private IEnumerator ShieldRun()
    {
        while (!myCore.IsDead)
        {
            yield return new WaitForSeconds(4f);
            shield?.Activate();
            yield return new WaitForSeconds(4f);
            shield?.Deactivate();
        }
        shield?.Deactivate();
    }
    private bool isAlerting;
    public void TurnTesla(bool active)
    {
        if (active)
        {
            if (isAlerting) return;
            isAlerting = true;
            if (UI_Core._)
            {
                UI_Core._.track.DoAlert(teslaReadyTime, Color.red);
            }

            teslaTween = DOVirtual.DelayedCall(teslaReadyTime, () =>
            {
                TeslaGun.enabled = true;
            });
            TeslaGun.trackTarget = carCore.transform;
        }
        else
        {
            if (UI_Core._)
            {
                UI_Core._.track.alertImage.fillAmount = 0;
            }


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

    public override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        TurnTesla(true);
    }

    public override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        TurnTesla(false);
    }
}
