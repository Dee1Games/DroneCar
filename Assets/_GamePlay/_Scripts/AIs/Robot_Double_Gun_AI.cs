using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_Double_Gun_AI : AI_Core
{
    [Title("Weapons")]
    public GunCore gun;
    public Shield shield;
    
    private void Start()
    {
        StartCoroutine(TurnRateStop());
        StartCoroutine(ShieldRun());
    }
    
    private IEnumerator ShieldRun()
    {
        while (!myCore.IsDead)
        {
            yield return new WaitForSeconds(4f);
            shield?.SetTarget(WeakPoint.CurrentActive.transform);
            shield?.Activate();
            yield return new WaitForSeconds(4f);
            shield?.Deactivate();
        }
        shield?.Deactivate();
    }

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        gun?.Activate();
    }

    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        gun?.Deactivate();
    }
    
}
