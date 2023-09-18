using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_HexShield_AI : AI_Core
{
    [Title("Weapons")] public MiniGun miniGun;

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        miniGun.Activate();
    }

    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        miniGun.Deactivate();
    }
}
