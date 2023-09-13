using UnityEngine;
public class CarCore : MonoBehaviour
{
    public static CarCore _;

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

    public void Restore()
    {
        Hp = MaxHp;
    }
    public void End(bool explode = true)
    {
        foreach (var aiCore in FindObjectsOfType<AI_Core>())
        {
            aiCore.OnEnd(this);
        }

        if (explode) vehicle.Explode();

        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
    }

    void Start()
    {
        vehicle = GetComponent<PlayerVehicle>();
        _ = this;
    }

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
    void Update()
    {
        
    }
}
