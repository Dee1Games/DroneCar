using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_AI : AI_Core
{
    public Gun gun;
    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        gun.Activate();
    }
    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        gun.Deactivate();
    }
}
