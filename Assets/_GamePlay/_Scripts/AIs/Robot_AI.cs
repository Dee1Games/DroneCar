using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_AI : AI_Core
{
    [Title("Weapons")]
    public IceGun iceGun;

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        DOVirtual.Float(animator.GetLayerWeight(1), 1, 1, f => HandsUp = f);
        iceGun.SetTarget(_core.transform);
        iceGun.Activate();
    }

    protected override void OnPlayerLost(CarCore _core)
    {
        DOVirtual.Float(animator.GetLayerWeight(1), 0, 1, f => HandsUp = f);
        base.OnPlayerLost(_core);
        iceGun.Deactivate();
    }
}
