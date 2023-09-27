using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class Extension
{
    public static void SafeKill(this Tween tween)
    {
        if (tween != null && tween.IsPlaying())
        {
            tween.Kill();
        }
    }
    public static bool SafeCheck(this Tween tween) => tween != null && tween.IsPlaying();
}
