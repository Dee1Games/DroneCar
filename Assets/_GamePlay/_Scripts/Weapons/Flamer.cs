using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.RaySensors;
using UnityEngine;

public class Flamer : MonoBehaviour
{
    // public bool stop;
    //
    // public float periodTimer = 4f;
    // public float flameTime = 2f;
    // private float currentTime;

    public float DPS = 4;

    public RaySensor raySensor;
    public Vector3 rayDirection;
    public ParticleSystem flamer;
    
    private void Start()
    {
        raySensor.onDetect.AddListener(c =>
        {
            if (c.transform.TryGetComponent(out CarCore carCore))
            {
                carCore.TakeDamage(DPS * Time.fixedDeltaTime);
            }
        });
    }

    public void FireOn()
    {
        flamer.Play(true);
        DOVirtual.Float(0f, 1f, .6f, f =>
        {
            raySensor.direction = f * rayDirection;
        });
    }

    public void FireOff()
    {
        flamer.Stop(true);
        DOVirtual.Float(1f, 0f, .6f, f =>
        {
            raySensor.direction = f * rayDirection;
        });
    }
}
