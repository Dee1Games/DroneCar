using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public static Monster _;
    public List<WeakPoint> weakPoints;
    
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Transform com;

    /// <summary>
    /// Auto property health for better managing
    /// </summary>
    public float Health
    {
        get => health;
        set
        {
            health = value;
            OnHealthChange?.Invoke(health, data.Health);
            UserManager.Instance.SetMonsterHealth(health/data.Health);
        }
    }

    public static System.Action<float, float> OnHealthChange;

    private MonsterData data;



    public int weakPointCount = 3;


    [Button("WeakPoints Setup")]
    public void FindWeakPoints()
    {
        weakPoints = GetComponentsInChildren<WeakPoint>().ToList();
        foreach (var weakPoint in weakPoints)
        {
            weakPoint.FindCore();
        }
    }

    public void Init(MonsterData data)
    {
        this.data = data;
        Health = data.Health*UserManager.Instance.Data.MonsterHealth;
        _ = this;

        FindWeakPoints();
        weakPoints.ForEach(w => w.gameObject.SetActive(false));
        RandomActive();
        weakPoints = weakPoints.Where(w => w.gameObject.activeInHierarchy).OrderBy(w => w.index).ToList();
    }

    private int count;
    private WeakPoint _wP;
    private void RandomActive()
    {
        if (count == weakPointCount) return;

        _wP = weakPoints[Random.Range(0, weakPoints.Count)];
        
        if (_wP.gameObject.activeInHierarchy)
        {
            RandomActive();
            return;
        }
        
        count++;
        _wP.Init(count);
        RandomActive();
    }
    
    public bool IsDead => (health <= 0f);
    public Vector3 GetCOMPos()
    {
        return com.position;
    }
}
