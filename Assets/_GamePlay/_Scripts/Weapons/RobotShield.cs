using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class RobotShield : Packable
{
    private static readonly int PackH = Animator.StringToHash("pack_H");
    public void SetPackHalfway(bool value)
    {
        _animator.SetBool(PackH, value);
    }
}
