using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float health;

    public static System.Action<float, float> OnHealthChange;

    public void Init()
    {
        health = maxHealth;
        OnHealthChange?.Invoke(health, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        OnHealthChange?.Invoke(health, maxHealth);
    }
}
