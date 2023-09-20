using System.Collections;
using System.Collections.Generic;
using RaycastPro.Casters;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_Double_Gun_AI : AI_Core
{
    [Title("Weapons")]
    public Gun gun;
    
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
