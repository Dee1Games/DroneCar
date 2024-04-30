using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    
    public VehicleConfig Config;
    
    [SerializeField] private VehicleID ID;
    [SerializeField] private float convertHeight = 2f;
    [SerializeField] private float rotationLerp = 2f;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject[] visuals;
    [SerializeField] private MMFeedbacks explodeFeedback;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float limitX;
    [SerializeField] private float limitY;
    [SerializeField] private ParticleSystem upgradeParticle;
    [SerializeField] private ParticleSystem downgradeParticle;
    [SerializeField] private Animator bombAnim;
    [SerializeField] private Animator turboAnim;
    [SerializeField] private Animator[] gunAnims;

    
    private List<UpgradeLevel> upgrades;
    private float acceleration;
    private float reverseAcceleration;
    private float maxSpeed;
    private float handeling;
    private float bomb;
    public float Bomb => bomb;
    
    private float gun;
    private float jumpForce;
    private Dictionary<UpgradeType, Dictionary<int, List<UpgradeItem>>> upgradeItems;
    private Dictionary<UpgradeType, LevelIndicatorUI> levelsUI;
    private GunExitPoint[] gunExitPoints;
    private int lastGunIndex;
    private float lastTimeShooting;
    
    public static System.Action OnExploded;
    public static System.Action OnTookDamage;

    public CarCore Core;
    public bool IsActive
    {
        get;
        private set;
    }
    
    
    private bool isHovering;
    private bool isChanging;
    
    private float currentSpeed;

    private Rigidbody rigidBody;
    public Rigidbody RigidBody => rigidBody;
    
    private Vector3 direction;


    private float fireRateMultiplyer;
    private float lastInputTouchTime;

    private float lifeTimer;

    public float CurrentLifTimeLeft => lifeTimer / Config.LifeTime;

    #region Cached_Animatios

    private static readonly int Reset = Animator.StringToHash("reset");
    private static readonly int Showcase = Animator.StringToHash("showcase");
    private static readonly int Hover = Animator.StringToHash("hover");

    #endregion


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        Core = GetComponent<CarCore>();

        levelsUI = new Dictionary<UpgradeType, LevelIndicatorUI>();
        List<LevelIndicatorUI> allLevelUIs = GetComponentsInChildren<LevelIndicatorUI>(true).ToList();
        foreach (LevelIndicatorUI levelUI in allLevelUIs)
        {
            levelsUI.Add(levelUI.Type, levelUI);
        }
    }

    public void InitPlayMode()
    {
        lifeTimer = Config.LifeTime;
        fireRateMultiplyer = 1f;
        lastInputTouchTime = -1f;
        upgrades = UserManager.Instance.GetUpgradeLevels(ID);
        anim.SetTrigger(Reset);
        Core.Restore();
        Core.hitedMonster = false;
        GetUpgradeValues();
        ShowUpgradeVisuals();
        IsActive = true;
        isHovering = false;
        isChanging = false;
        CameraController.Instance.SetTarget(transform);
        SetVisualsVisibility(true);
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        input.Init();

        foreach (LevelIndicatorUI levelUI in levelsUI.Values)
        {
            levelUI.gameObject.SetActive(false);
        }

        lastTimeShooting = Time.timeSinceLevelLoad;
    }
    
    public void InitShowCaseMode()   
    {
        upgrades = UserManager.Instance.GetUpgradeLevels(ID);
        GetUpgradeValues();
        ShowUpgradeVisuals();
        IsActive = false;
        isHovering = false;
        SetVisualsVisibility(true);
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        anim.SetTrigger(Showcase);
        
        
        foreach (LevelIndicatorUI levelUI in levelsUI.Values)
        {
            levelUI.gameObject.SetActive(true);
        }
    }

    public void PlayUpgradeAnim(UpgradeType t)
    {
        if (t == UpgradeType.Bomb)
        {
            bombAnim.SetTrigger("bounce");
        } else if (t == UpgradeType.Gun)
        {
            foreach (Animator anim in gunAnims)
            {
                anim.SetTrigger("bounce");
            }
        }  else if (t == UpgradeType.Turbo)
        {
            turboAnim.SetTrigger("bounce");
        }  
    }

    public void ShowUpgradeVisuals()
    {
        upgradeItems = new Dictionary<UpgradeType, Dictionary<int, List<UpgradeItem>>>();
        List<UpgradeItem> allItems = GetComponentsInChildren<UpgradeItem>(true).ToList();

        foreach (UpgradeItem item in allItems)
        {
            if (!upgradeItems.ContainsKey(item.Type))
            {
                upgradeItems.Add(item.Type, new Dictionary<int, List<UpgradeItem>>());
            }
            if (!upgradeItems[item.Type].ContainsKey(item.Level))
            {
                upgradeItems[item.Type].Add(item.Level, new List<UpgradeItem>());
            }
            upgradeItems[item.Type][item.Level].Add(item);
            item.gameObject.SetActive(false);
        }

        foreach (UpgradeLevel upgradeLevel in upgrades)
        {
            try
            {
                List<UpgradeItem> list = upgradeItems[upgradeLevel.Type][upgradeLevel.Level];
                foreach (UpgradeItem item in list)
                {
                    item.gameObject.SetActive(true);
                    levelsUI[item.Type].SetLevel(item.Level);
                }
            }
            catch {
                
            }
        }

        gunExitPoints = GetComponentsInChildren<GunExitPoint>();
    }

    private void Update()
    {
        if (!IsActive || !GameManager.Instance.IsPlaying())
            return;
        
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0f)
        {
            Core.Die();
            return;
        }
        
        if (transform.position.z < Config.LimitZ)
        {
            Core.Die();
            return;
        }
        
        
        if (isChanging)
            return;

        

        if (Input.GetMouseButtonDown(0))
        {
            lastInputTouchTime = Time.timeSinceLevelLoad;
            fireRateMultiplyer = 3f;
        }
        else if (Time.timeSinceLevelLoad - lastInputTouchTime > 0.2f)
        {
            fireRateMultiplyer = 1f;
        }

        float height = getDistanceToGround();

        if (!isHovering && height > convertHeight)
            ConvertToDrone(false);
        
        if (isHovering && height < convertHeight)
            ConvertToCar();
        
        if(!isHovering && input.SwipeUp)
            ConvertToDrone();
        
        if (isHovering)
        {
            float inputForward = 1f;
            float speedDiff = (maxSpeed * inputForward) - currentSpeed;
            if (speedDiff > 0)
            {
                float delta = acceleration * Time.deltaTime;
                if (delta + currentSpeed <= maxSpeed)
                    currentSpeed += delta;
                else
                    currentSpeed = maxSpeed;
            }
            else {
                float delta = -acceleration * Time.deltaTime;
                if (delta + currentSpeed >= 0f)
                    currentSpeed += delta;
                else
                    currentSpeed = 0f;
            }
            
            if (input.Hold)
            {
                transform.Rotate(Vector3.up, input.Right*handeling);
                transform.Rotate(Vector3.right, -input.Up*handeling);
            }
            
            if(Physics.Raycast(transform.position, transform.forward, 3f, LayerMask.GetMask("Props")))
            {
                currentSpeed = 0f;
                
                Core.Die();
                //UIManager.Instance.ShowScreen(UIScreenID.EndRun);

                return;
            }
            
            if (transform.position.magnitude > LevelManager.Instance.GetSpaceLimit())
            {
                //currentSpeed = 0f;
            }
            // باف سرعت رو داخل کور اصلی حساب میکنه

            float yy = Mathf.Clamp(transform.eulerAngles.y, 180f - 45f, 180f + 45f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yy, 0f);
            direction = new Vector3(input.Right, 0f, inputForward);
        }
        else
        {
            float speedDiff = (maxSpeed * input.Forward) - currentSpeed;
            if (speedDiff > 0)
            {
                float delta = acceleration * Time.deltaTime;
                if (delta + currentSpeed <= maxSpeed)
                    currentSpeed += delta;
                else
                    currentSpeed = maxSpeed;
            }
            else {
                float delta = -reverseAcceleration * Time.deltaTime;
                if (delta + currentSpeed >= 0f)
                    currentSpeed += delta;
                else
                    currentSpeed = 0f;
            }
            
            if(Physics.Raycast(transform.position, transform.forward, 3f, LayerMask.GetMask("Props")))
            {
                currentSpeed = 0f;
            }
            
            if (transform.position.magnitude > LevelManager.Instance.GetSpaceLimit())
            {
                //currentSpeed = 0f;
            }

            direction = new Vector3(input.JoystickX, 0f, input.Forward);
            float ang = input.JoystickX * handeling;
            transform.Rotate(Vector3.up, ang);
            float yy = Mathf.Clamp(transform.eulerAngles.y, 180f - 45f, 180f + 45f);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, yy, 0f), rotationLerp*Time.deltaTime);
        }

        if (CanShoot())
        {
            Shoot();
            lastTimeShooting = Time.timeSinceLevelLoad;
        }
        
        if (!UserManager.Instance.Data.SeenMoveTutorial)
        {
            if (currentSpeed == 0f)
            {
                TutorialManager.Instance.ShowMoveHint();
            }
            else
            {
                UserManager.Instance.SeenMoveTutorial();
                TutorialManager.Instance.Hide();
            }
        }
        
        if (!UserManager.Instance.Data.SeenFlyTutorial && UserManager.Instance.Data.SeenMoveTutorial && currentSpeed>0f && transform.position.y<10f)
        {
            if (!isHovering)
            {
                TutorialManager.Instance.ShowFlyHint();
            }
            else
            {
                UserManager.Instance.SeenFlyTutorial();
                TutorialManager.Instance.Hide();
            }
        }

        if (isHovering)
        {
            // if (!Physics.Raycast(transform.position, transform.forward, 1000f, enemyLayer))
            // {
            //     TutorialManager.Instance.ShowHitGiantHint();
            // }
            // else
            {
                TutorialManager.Instance.Hide();
            }
        }
    }

    private bool CanShoot()
    {
        if (isHovering && Time.timeSinceLevelLoad - lastTimeShooting > (1f / (Config.FireRate*fireRateMultiplyer)) && gunExitPoints.Length!=0)
        {
            if (transform.position.z > Config.GunShootZ)
            {
                return false;
            }
            
            if (Config.AlwaysShoot)
            {
                return true;
            }
            else
            {
                if(Physics.Raycast(transform.position, transform.forward, 1000f, LayerMask.GetMask("Enemy")))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }

    private void Shoot()
    {
        ProjectileMoveScript projectile = ProjectilePoolManager.Instance.GetProjectile(upgrades.FirstOrDefault(U=>U.Type==UpgradeType.Gun).Level-1);
        projectile.transform.position = gunExitPoints[lastGunIndex].transform.position;
        projectile.transform.forward = gunExitPoints[lastGunIndex].transform.forward;
        projectile.Init(gun);
        lastGunIndex = (lastGunIndex + 1) % gunExitPoints.Length;
    }

    private void FixedUpdate()
    {
        if (!IsActive || isChanging || !GameManager.Instance.IsPlaying())
            return;
        
        if (isHovering)
        {
            Vector3 dir = transform.forward;
            if (transform.position.x <= -limitX && transform.forward.x<0)
            {
                dir.x = 0f;
            }
            if (transform.position.x >= limitX && transform.forward.x>0)
            {
                dir.x = 0f;
            }
            rigidBody.velocity = dir * Core.ApplySpeedBuff(currentSpeed);
            rigidBody.angularVelocity = Vector3.zero;
        }
        else
        {
            Vector3 dir = transform.forward;
            if (transform.position.x <= -limitX && transform.forward.x<0)
            {
                dir.x = 0f;
            }
            if (transform.position.x >= limitX && transform.forward.x>0)
            {
                dir.x = 0f;
            }
            Vector3 v = dir * currentSpeed;
            v.y = rigidBody.velocity.y;
            rigidBody.velocity = v;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }
    
    public void PlayUpgradeParticle()
    {
        upgradeParticle.Play();
    }
    
    public void PlayDowngradeParticle()
    {
        downgradeParticle.Play();
    }

    public void Explode()
    {
        explodeFeedback.PlayFeedbacks();
    }

    public void Deactivate()
    {
        IsActive = false;
        CameraController.Instance.SetTarget(null);
        SetVisualsVisibility(false);
        anim.SetBool(Hover, false);
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        OnExploded?.Invoke();
    }

    public void SetVisualsVisibility(bool value)
    {
        foreach (GameObject visual in visuals)
        {
            visual.SetActive(value);
        }
    }

    // I Move collision code to CarCore for control damage System.

    private void ConvertToCar()
    {
        StartCoroutine(convertToCar());
    }

    private IEnumerator convertToCar()
    {
        isChanging = true;
        isHovering = false;
        anim.SetBool(Hover, false);
        rigidBody.useGravity = true;
        while (getDistanceToGround()>1f)
        {
            yield return new WaitForEndOfFrame();
        }
        isChanging = false;
    }
    
    private void ConvertToDrone(bool jump = true)
    {
        StartCoroutine(convertToDrone(jump));
    }
    
    private IEnumerator convertToDrone(bool jump = true)
    {
        isChanging = true;
        isHovering = true;
        anim.SetBool(Hover, true);
        if(jump) 
            rigidBody.AddForce(Vector3.up*jumpForce, ForceMode.VelocityChange);

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0f, startRot.eulerAngles.y, 0f);
        float timer = 0f;
        float dur = 1f;
        //rigidbody.velocity = Vector3.zero;
        //rigidbody.useGravity = false;
        rigidBody.angularVelocity = Vector3.zero;
        Vector3 startV = rigidBody.velocity;
        while (timer<dur)
        {
            //rigidbody.velocity = new Vector3(startV.x, startV.y*(1-(timer/dur)), startV.z);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, timer/dur);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            rigidBody.angularVelocity = Vector3.zero;
        }
        
        rigidBody.useGravity = false;
        isChanging = false;

        //yield return new WaitForSeconds(999);
        
    }

    private bool isOnGround()
    {
        return getDistanceToGround() < 0.1f;
    }
    
    public LayerMask groundLayer;

    private float getDistanceToGround()
    {
        Ray ray = new Ray(pivot.position+(Vector3.up*10f), Vector3.down);
        if(Physics.Raycast(ray, out var hit, 9999f, groundLayer.value))
        {
            return hit.distance-10f;
        }

        return 9999f;
    }
    
    private void updateWheelVisuals(WheelCollider wheelCollider, Transform wheelMesh)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }

    public bool SetUpgrade(UpgradeType type, int level)
    {
        if (level < 0)
        {
            level = 0;
        }
        if (Config.GetUpgradeConfig(type).maxLevel < level)
        {
            level = Config.GetUpgradeConfig(type).maxLevel;
        }   
        int upgradeIndex = -1;
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].Type == type)
            {
                // if (upgrades[i].Level >= level)
                // {
                //     return false;
                // }
                // else
                {
                    upgrades[i].Level = level;
                    UserManager.Instance.SetUpgradeLevel(ID, type, level);
                    GetUpgradeValues();
                    return true;
                }
            }
        }

        return false;
    }
    
    public int GetUpgradeLevel(UpgradeType type)
    {
        int upgradeIndex = -1;
        for (int i = 0; i < upgrades.Count; i++)
        {
            if (upgrades[i].Type == type)
            {
                return upgrades[i].Level;
            }
        }

        return 0;
    }
    
    private void GetUpgradeValues ()
    {
        acceleration = Config.Acceleration;
        reverseAcceleration = Config.ReverseAcceleration;
        maxSpeed = Config.MaxSpeed;
        handeling = Config.Handeling;
        bomb = Config.Bomb;
        gun = Config.Gun;
        jumpForce = Config.JumpForce;
        
        foreach (UpgradeLevel upgradeLevel in upgrades)
        {
            UpgradeConfig upgrade = Config.GetUpgradeConfig(upgradeLevel.Type);

            acceleration = Mathf.Max(acceleration, upgrade.GetAcceleration(upgradeLevel.Level));
            reverseAcceleration = Mathf.Max(reverseAcceleration, upgrade.GetReverseAcceleration(upgradeLevel.Level));
            maxSpeed = Mathf.Max(maxSpeed, upgrade.GetMaxSpeed(upgradeLevel.Level));
            handeling = Mathf.Max(handeling, upgrade.GetHandling(upgradeLevel.Level));
            bomb = Mathf.Max(bomb, upgrade.GetBomb(upgradeLevel.Level));
            gun = Mathf.Max(gun, upgrade.GetGun(upgradeLevel.Level));
            jumpForce = Mathf.Max(jumpForce, upgrade.GetJumpForce(upgradeLevel.Level));
        }
        
        acceleration *= Config.SpeedMultiplyer;
        reverseAcceleration *= Config.SpeedMultiplyer;
        maxSpeed *= Config.SpeedMultiplyer;
    }

    public void pointToMonster()
    {
        if(GameManager.Instance.Monster == null)
            return;

        Transform t = GameManager.Instance.Monster.com.transform;
        Vector3 f = t.position - transform.position;
        f = f.normalized;
        transform.forward = f;
    }
}
