using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Bullet_Core : MonoBehaviour
{
    public float scaleTweenDuration = .7f;
    public float intoGroundDuration = 4f;
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOPunchScale(Vector3.one, scaleTweenDuration);
    }
    void Update()
    {
        
    }

    public void OnEnd()
    {
        transform.DOMoveY(-4, intoGroundDuration);
    }
}
