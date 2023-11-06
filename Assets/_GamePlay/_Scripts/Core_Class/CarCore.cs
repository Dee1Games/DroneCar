using System;
using System.Linq;
using DG.Tweening;
using RaycastPro;
using RaycastPro.RaySensors;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public interface IHitable
{
    void OnHit(CarCore core, float damage);
}
public class CarCore : MonoBehaviour
{
    public static CarCore _;
    
    [Tooltip("Forward Ray for detecting objects")]
    public RaySensor FRay;
    
    [SerializeField] private float hp = 100f;
    [SerializeField] private float maxHp = 100f;

    [Header("Debug")] public bool debug;

    public bool hitedMonster;


    #region Buff System
    
    private Tween slowMotion;
    [SerializeField] private AnimationCurve slowMotionCurve = AnimationCurve.EaseInOut(0, 0f, 1, 1);

    public ParticleSystem iceBuff;

    private ParticleSystem currentIceBuff;

    #endregion

    #region Buff System
    public float ApplySpeedBuff(float currentSpeed)
    {
        if (slowMotion.SafeCheck())
        {
            var pos = slowMotion.position / slowMotion.Duration();
            return currentSpeed * slowMotionCurve.Evaluate(pos);
        }
        return currentSpeed;
    }
    public static void BuffPlay(ref Tween buff, float time = 6f)
    {
        if (buff.SafeCheck())
        {
            buff.Restart();
            return;
        }
        buff = DOVirtual.DelayedCall(time, () => { Debug.Log($"{nameof(buff)} has been finished!"); });
    }
    #endregion

    public void ApplySlowMotion(float duration = 3f) 
    {
        currentIceBuff?.Play(true);
        
        if (slowMotion.SafeCheck())
        {
            slowMotion.Restart();
            return;
        }
        slowMotion = DOVirtual.DelayedCall(duration, () => currentIceBuff?.Stop(true));

        BuffPlay(ref slowMotion, duration);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BuffPlay(ref slowMotion, 6);
        }
    }

    public float Hp
    {
        get => hp;
        set
        {
            if (value < hp)
            {
                PlayerVehicle.OnTookDamage();
            }
            hp = Mathf.Clamp(value, 0, maxHp);
            UI_Core._.carHealth.UpdateHealthUI(hp, maxHp);
            if (hp <= 0)
            {
                End(true);
                CameraZoomOut();
                //UIManager.Instance.ShowScreen(UIScreenID.EndRun);
            }
        }
    }

    public float MaxHp
    {
        get => maxHp;
        set { maxHp = value; }
    }

    public PlayerVehicle vehicle;


    private void OnEnable()
    {
        // Always Singleton to last
        _ = this;
    }

    void Start()
    {
        vehicle = GetComponent<PlayerVehicle>();
        _colliders = GetComponentsInChildren<Collider>();

        if (!currentIceBuff)
        {
            currentIceBuff = Instantiate(iceBuff, transform.position, Quaternion.identity, transform);
        }
        
        if (!FRay)
        {
            FRay = GetComponentInChildren<RaySensor>();
        }
        FRay?.onBeginDetect.AddListener(OnRayHit);
    }
    
    
    public void OnRayHit(RaycastHit hit)
    {
        if(!GameManager.Instance.Player.IsActive)
            return;
        
        Debug.Log("F Ray Hit on: "+hit.transform.name);
        var hitable = hit.transform.GetComponentInParent<IHitable>();
        if (hitable != null)
        {
            End(false);
            UserManager.Instance.SeenFlyTutorial();
            hitable.OnHit(this, vehicle.Bomb);
            hitedMonster = true;
            CameraZoomOut();
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
    public void End(bool runFailed, bool explode = true)
    {
        if (!vehicle.IsActive)
            return;
        
        FRay.enabled = false;

        
        if (explode)
        {
            vehicle.Explode();
        }
        vehicle.Deactivate();
        
        CollidersActivate(false);

        if (runFailed)
        {
            Debug.Log($"Run {UserManager.Instance.Data.Run} Failed");
            try
            {
                //SupersonicWisdom.Api.NotifyLevelFailed(UserManager.Instance.Data.Run, null);
            } catch {}
        }
        else
        {
            Debug.Log($"Run {UserManager.Instance.Data.Run} Completed");
            try
            {
                //SupersonicWisdom.Api.NotifyLevelCompleted(UserManager.Instance.Data.Run, null);
            } catch {}
        }
        
        UserManager.Instance.NextRun();
    }

    public void CameraZoomOut()
    {
        CameraController.Instance.TakeLongShot(transform.position, (transform.position-Camera.main.transform.position).normalized);
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
