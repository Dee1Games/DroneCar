using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicatorReturnToPool : MonoBehaviour
{
    [SerializeField] private float returnTime;

    private float activateTime;

    private void OnEnable()
    {
        activateTime = Time.timeSinceLevelLoad;
    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad - activateTime > returnTime)
        {
            gameObject.SetActive(false);
        }
    }
}
