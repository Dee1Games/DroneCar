using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine;

public class Robot_Saw_AI : AI_Core
{
    public float sawPeriodTime = 6f;
    public float minimumDistanceThreshold = 15;
    public float sharpness = 15F;
    
    public Saw saw;

    private void Start()
    {
        StartCoroutine(JigSawPeriod());
    }

    private IKEffector LeftHand => myCore.fullBodyBipedIK.solver.leftHandEffector;
    
    private IEnumerator JigSawPeriod()
    {
        yield return new WaitForSeconds(1F);
        while (!myCore.IsDead)
        {
            saw.SetPack(true);
            DOVirtual.Float(saw.SawSpeed, 0, 1f, v =>
            {
                saw.SawSpeed = v;
                saw.trailRenderer.widthMultiplier = v;
            });

            
            DOVirtual.Float(LeftHand.positionWeight, 0f, 1f, v =>
            {
                LeftHand.positionWeight = v;
            }).OnComplete(() => LeftHand.target = null);

            yield return new WaitForSeconds(1F);
        
            // On Catch Car
            if (carCore)
            {
                saw.SetPack(false);

                DOVirtual.Float(saw.SawSpeed, Random.Range(1f, 2f), 1f, v =>
                {
                    saw.SawSpeed = v;
                    saw.trailRenderer.widthMultiplier = v;
                });
            
                var target = carCore.transform;
                DOVirtual.Float(LeftHand.positionWeight, 1f, 1f, v =>
                {
                    LeftHand.positionWeight = v;
                });
            
                var progress = 0f;
                while (progress  <= sawPeriodTime && Distance <= minimumDistanceThreshold)
                {
                    var pos = target.transform.position + (transform.position - target.position).normalized * 14f;
                    LeftHand.position = Vector3.Lerp(LeftHand.position, pos, 1-Mathf.Exp(-sharpness *Time.deltaTime));
                    yield return new WaitForSeconds(Time.deltaTime);
                    progress += Time.deltaTime;
                }
            }
        }
    }

    protected override void OnPlayerFound(CarCore _core)
    {
        base.OnPlayerFound(_core);
    }

    protected override void OnPlayerLost(CarCore _core)
    {
        base.OnPlayerLost(_core);
    }
}
