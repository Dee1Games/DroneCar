using System;
using RaycastPro.RaySensors;
using UnityEngine;
public class CarCore : MonoBehaviour
{
    public static CarCore _;

    [Tooltip("Forward Ray for detecting objects")]
    public RaySensor FRay;

    [SerializeField] private float hp = 100f;
    [SerializeField] private float maxHp = 100f;

    [Header("Debug")] public bool debug;
    
    public float Hp
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            UI_Core._.carHealth.UpdateHealthUI(hp, maxHp);
            if (hp <= 0)
            {
                End();
            }
        }
    }

    public float MaxHp
    {
        get => maxHp;
        set { maxHp = value; }
    }

    public PlayerVehicle vehicle;

    public Sprite targetOn;
    
    void Start()
    {
        vehicle = GetComponent<PlayerVehicle>();
        _colliders = GetComponentsInChildren<Collider>();
        _ = this;

        if (!FRay) FRay = GetComponentInChildren<RaySensor>();

        FRay.onBeginDetect.AddListener(OnRayHit);
    }

    public void OnRayHit(RaycastHit hit)
    {
        Debug.Log("F Ray Hit on: "+hit.transform.name);
        var hitable = hit.transform.GetComponentInParent<IHitable>();
        if (hitable != null)
        {
            hitable.OnHit(this, vehicle.Bomb);
            End();
        }
    }
    private Collider[] _colliders = Array.Empty<Collider>();
    public void CollidersActivate(bool enable)
    {
        foreach (var col in _colliders)
        {
            col.enabled = enable;
        }
    }

    public void Restore()
    {
        FRay.enabled = true;
        Hp = MaxHp;
        CollidersActivate(true);
    }
    public void End(bool explode = true)
    {
        FRay.enabled = false;
        foreach (var aiCore in FindObjectsOfType<AI_Core>())
        {
            aiCore.OnEnd(this);
        }

        if (explode) vehicle.Explode();

        CollidersActivate(false);
    }

    private IHitable hitable;
    
    void OnBullet(RaycastPro.Bullets.Bullet bullet)
    {
        TakeDamage(bullet.damage);
    }

    public void TakeDamage(float amount)
    {
        Hp -= amount;
        if (debug)
        {
            Debug.Log($"current car Hp is: {Hp}");
        }
    }
}
