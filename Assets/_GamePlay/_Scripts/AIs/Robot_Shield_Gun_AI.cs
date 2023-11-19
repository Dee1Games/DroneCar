using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_Shield_Gun_AI : AI_Core
{
    [Title("Weapons")]
    public GunCore gun;


    public override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        gun?.Activate();
    }
    
    public override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        gun?.Deactivate();
    }
}
