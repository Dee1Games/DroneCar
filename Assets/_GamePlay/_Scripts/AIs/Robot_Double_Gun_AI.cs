using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_Double_Gun_AI : AI_Core
{
    [Title("Weapons")]
    public Gun gun;

    [SerializeField] private float turnStopDelay = 8;
    [SerializeField] private float turnStopTime = 4;
    private void Start()
    {
        StartCoroutine(TurnRateStop());
    }

    public IEnumerator TurnRateStop()
    {
        while (true)
        {
            yield return new WaitForSeconds(turnStopDelay);
            allowTurning = false;
            yield return new WaitForSeconds(turnStopTime);
            allowTurning = true;
        }

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
