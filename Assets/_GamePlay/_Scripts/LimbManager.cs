using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbManager : MonoBehaviour
{
    public static LimbManager _;
    
    public ParticleSystem dismemberEffect;
    public Material dismemberMaterial;

    public static ParticleSystem DismemberEffect => _.dismemberEffect;

    public static Material DismemberMaterial => _.dismemberMaterial;
    void Start()
    {
        _ = this;
    }

    public static void ExplodeEffect(Transform target)
    {
        _.StartCoroutine(IExplodeEffect(target));
    }

    private static IEnumerator IExplodeEffect(Transform target)
    {
        for (int i = 0; i < 18; i++)
        {
            Instantiate(DismemberEffect, target.position+Random.insideUnitSphere*Random.Range(2f, 15f), Quaternion.identity);
            yield return new WaitForSeconds(.15f);
        }
    }

}
