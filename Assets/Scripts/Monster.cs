using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //[SerializeField] private Animator animator;
    [SerializeField] private List<WeakPoint> weakPoints;
    
    [SerializeField] private float health = 100f;


    public static System.Action<float, float> OnHealthChange;

    private MonsterData data;

    public bool IsDead => (health <= 0f);

    public void Init(MonsterData data)
    {
        this.data = data;
        health = data.Health;
        OnHealthChange?.Invoke(health, data.Health);
    }

    public void TakeDamage(float damage, Vector3 pos)
    {
        bool weakPoint = false;

        Collider[] colls = Physics.OverlapSphere(pos, 5f);
        foreach (Collider coll in colls)
        {
            WeakPoint point  = coll.gameObject.GetComponent<WeakPoint>();
            if (point != null)
            {
                point.Hit();
                weakPoint = true;
                damage *= 2f;
                coll.gameObject.SetActive(false);
                Debug.Log("Hit Weak Point");
            }
        }
        
        health -= damage;
        OnHealthChange?.Invoke(health, data.Health);

        if (health <= 0f)
        {
            //animator.SetTrigger("die");
        }
    }
}
