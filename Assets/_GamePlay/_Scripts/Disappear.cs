using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public static float Delay = 8f;
    public static float Duration = 7f;
    
    void Start()
    {
        DOVirtual.DelayedCall(Delay, Run);
    }

    public void Run()
    {
        transform.DOMove(Vector3.down * 20, Duration).OnComplete(() => Destroy(gameObject));
    }
}
