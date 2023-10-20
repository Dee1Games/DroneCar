using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_Double_Gun_AI : AI_Core
{
    [Title("Weapons")]
    public GunCore gun;
    public Shield shield;
    
    [SerializeField] private float turnStopDelay = 8;
    [SerializeField] private float turnStopTime = 4;
    private void Start()
    {
        StartCoroutine(TurnRateStop());
        StartCoroutine(ShieldRun());
        
        
    }
    
    private IEnumerator ShieldRun()
    {
        while (true)
        {
            yield return new WaitForSeconds(4f);
            shield?.SetTarget(WeakPoint.CurrentActive.transform);
            shield?.Activate();
            yield return new WaitForSeconds(4f);
            shield?.Deactivate();
        }
        shield?.Deactivate();
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

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        gun?.Activate();
    }

    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        gun?.Deactivate();
    }
    
}
