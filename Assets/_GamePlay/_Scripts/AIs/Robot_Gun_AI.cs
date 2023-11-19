using System.Collections;
using RaycastPro.RaySensors;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_Gun_AI : AI_Core
{
    [Title("Weapons")]
    public GunCore gun;

    [SerializeField] private float turnStopDelay = 8;
    [SerializeField] private float turnStopTime = 4;

    public TargetRay targetRay;

    public float laserDamage;

    private Transform laserTarget;
    private void Start()
    {
        StartCoroutine(TurnRateStop());
        StartCoroutine(LaserTime());

        
        laserTarget = new GameObject().transform;
        laserTarget.name = "Laser Target";
        laserTarget.parent = myCore.monster.transform;
        
        targetRay.target = laserTarget;
        
        targetRay.onDetect.AddListener(hit =>
        {
            if (carCore && hit.transform == carCore.transform)
            {
                carCore.TakeDamage(Time.deltaTime * laserDamage*targetRay.Influence);
            }
        });
    }

    public IEnumerator LaserTime()
    {
        while (true)
        {
            if (carCore)
            {
                laserTarget.position = carCore.transform.position + carCore.transform.forward*6f;
            }
            
            targetRay.gameObject.SetActive(carCore);

            yield return new WaitForSeconds(4);
        }
    }

    public IEnumerator TurnRateStop()
    {
        while (true)
        {
            yield return new WaitForSeconds(turnStopDelay);
            animator.SetFloat(TurnAngle, 0);
            allowTurning = false;
            yield return new WaitForSeconds(turnStopTime);
            allowTurning = true;
        }
    }

    public override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        gun?.Activate();
        gun.caster.trackTarget = _core.transform;
    }

    public override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        gun?.Deactivate();
    }
}
