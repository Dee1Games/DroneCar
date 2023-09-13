using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_AI : AI_Core
{
    [Title("Weapons")]
    public IceGun iceGun;

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        iceGun.SetTarget(_core.transform);
        iceGun.Activate();
    }

    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        iceGun.Deactivate();
    }
}
