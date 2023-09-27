using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot_Saw_AI : AI_Core
{
    public Saw saw;

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
    }

    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
    }
}
