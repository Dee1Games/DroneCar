using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot_HexShield_AI : AI_Core
{
    [Title("Weapons")]
    public MiniGun miniGun;

    public RobotShield shield;
    public Dish dish;

    protected Vector3 CarDirection => Vector3.ProjectOnPlane(carCore.transform.position - transform.position, transform.up);
    protected float Dot => Vector3.Dot(transform.forward, CarDirection.normalized);

    protected void Start()
    {
        DOVirtual.DelayedCall(1f, PulseUpdate).SetLoops(400);
        StartCoroutine(TurnRateStop());
        StartCoroutine(DishPeriod());
    }

    private IEnumerator DishPeriod()
    {
        yield return new WaitForSeconds(4f);
        dish.Unpack();
        yield return new WaitForSeconds(4f);
        dish.Pack();
    }

    protected void PulseUpdate()
    {
        if (!carCore) return;
        Debug.Log(Dot);
        shield.SetPack(Dot < .5f);
    }

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
        miniGun.Activate();
    }

    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
        miniGun.Deactivate();
    }
}
