using System.Collections;
using System.Collections.Generic;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using UnityEngine;

public class TeslaBot_AI : AI_Core
{
    [Header("Guns")]
    public AdvanceCaster TeslaGun;
    public TargetRay laserGun;

    public float TeslaTimer = 3f;
    public float currentTeslaTimer;
    public override void Update()
    {
        base.Update();

        if (TeslaGun.trackTarget)
        {
            if (currentTeslaTimer < TeslaTimer)
            {
                currentTeslaTimer += Time.deltaTime;
            }
            else
            {
                TeslaGun.enabled = true;
            }
        }
        UIManager.Instance.SetGiantGunTimer(TeslaGun.ammo.currentReloadTime);
    }

    public override void Active(bool phase)
    {
        base.Active(phase);
        TeslaGun.enabled = phase;
    }
    protected override void OnPlayerFound(CarCore vehicle)
    {
        base.OnPlayerFound(vehicle);
        TeslaGun.trackTarget = vehicle.transform;
        laserGun.target = vehicle.transform;
        laserGun.gameObject.SetActive(true);
        currentTeslaTimer = 0;
    }
    protected override void OnPlayerLost(CarCore vehicle)
    {
        base.OnPlayerLost(vehicle);
        TeslaGun.trackTarget = null;
        laserGun.gameObject.SetActive(false);
        laserGun.target = null;
        TeslaGun.enabled = false;
    }
    
    
}
