using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class Extension
{
    public static void SafeKill(this Tween tween)
    {
        if (SafeCheck(tween))
        {
            tween.Kill();
        }
    }
    public static bool SafeCheck(this Tween tween) => tween != null && tween.IsPlaying();
}
