using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //[SerializeField] private Animator animator;
    [SerializeField] private List<WeakPoint> weakPoints;
    
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHealth = 100f;

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
        }
    }

    public static System.Action<float, float> OnHealthChange;

    private MonsterData data;

    public bool IsDead => (health <= 0f);

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
        Health = data.Health;
    }
}
