using System.Collections;
using System.Collections.Generic;
using RaycastPro.Casters;
using RaycastPro.RaySensors;
using UnityEngine;

public class TeslaBot_AI : AI_Core
{
    /// <summary>
    /// Main Object for Follow
    /// </summary>
    public Transform carTransform;

    [Header("Guns")]
    public AdvanceCaster TeslaGun;
    public TargetRay laserGun;

    public override void Active(bool phase)
    {
        base.Active(phase);
        TeslaGun.enabled = phase;
    }
    protected override void OnPlayerFound(PlayerVehicle vehicle)
    {
        base.OnPlayerFound(vehicle);
        TeslaGun.trackTarget = vehicle.transform;
        laserGun.target = vehicle.transform;
        laserGun.gameObject.SetActive(true);
        TeslaGun.enabled = true;
    }
    protected override void OnPlayerLost(PlayerVehicle vehicle)
    {
        base.OnPlayerLost(vehicle);
        TeslaGun.trackTarget = null;
        laserGun.gameObject.SetActive(false);
        laserGun.target = null;
        TeslaGun.enabled = false;
        playerVehicle = null;
        sightDetector.DetectedColliders.Clear();
    }
}
