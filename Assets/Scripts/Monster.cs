using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using SupersonicWisdomSDK;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<WeakPoint> weakPoints;
    
    private float health;
    private float maxHealth;

    public static System.Action<float, float> OnHealthChange;


    public bool IsDead => (health <= 0f);

    public void Init()
    {
        this.maxHealth = LevelManager.Instance.GetCurrentMonsterHealth();
        health = UserManager.Instance.Data.MonsterHealth*maxHealth;
        OnHealthChange?.Invoke(health, maxHealth);
    }

    public void OnRunStarted()
    {
        OnHealthChange?.Invoke(health, maxHealth);
    }

    public void TakeDamage(float damage, Vector3 pos, bool bullet)
    {
        if (!GameManager.Instance.Player.IsActive)
            return;
        
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
        GameManager.Instance.CurrentRunDamage += damage;
        UserManager.Instance.SetMonsterHealth(health/maxHealth);
        OnHealthChange?.Invoke(health, maxHealth);

        if (GameManager.Instance.Monster.IsDead)
        {
            CameraController.Instance.TakeLongShot(pos, GameManager.Instance.Player.transform.forward);
            if (bullet)
            {
                GameManager.Instance.Player.Deactivate();
            }
            else
            {
                GameManager.Instance.Player.Explode();
            }
            
            UserManager.Instance.NextLevel();
            animator.SetTrigger("die");
            Debug.Log($"Run {UserManager.Instance.Data.Run} Ended");
            try
            {
                SupersonicWisdom.Api.NotifyLevelCompleted(UserManager.Instance.Data.Run, null);
            }
            catch
            {
            }
            UserManager.Instance.NextRun();
        } else if (!bullet)
        {
            CameraController.Instance.TakeLongShot(pos, GameManager.Instance.Player.transform.forward);
            GameManager.Instance.Player.Explode();
        }
    }
}
