using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    public static PlayerVehicle Instance;
    
    public VehicleConfig Config;
    
    [SerializeField] private float convertHeight = 2f;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Transform pivot;
    [SerializeField] private GameObject[] visuals;
    [SerializeField] private MMFeedbacks explodeFeedback;
    [SerializeField] private List<UpgradeLevel> upgrades;

    private float acceleration;
    private float reverseAcceleration;
    private float maxSpeed;
    private float handeling;
    private float bomb;
    private float gun;
    private float jumpForce;
    
    public static System.Action OnExploded;

    public bool IsActive
    {
        get;
        private set;
    }
    
    
    private bool isHovering;
    private bool isChanging;
    
    private float currentSpeed;

    private Rigidbody rigidbody;
    private Animator animator;
    private Vector3 direction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    public void Init()
    {
        GetUpgradeValues();
        IsActive = true;
        isHovering = false;
        isChanging = false;
        CameraController.Instance.SetTarget(transform);
        SetVisualsVisibility(true);
        rigidbody.isKinematic = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        input.Init();
    }

    private void Update()
    {
        if (!IsActive || isChanging)
            return;

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
            }

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
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
            
            direction = new Vector3(input.JoystickX, 0f, input.Forward);
            transform.Rotate(Vector3.up, input.JoystickX*handeling);
        }
    }

    private void FixedUpdate()
    {
        if (!IsActive || isChanging)
            return;
        
        if (isHovering)
        {
            rigidbody.velocity = transform.forward * currentSpeed;
            rigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            Vector3 v = transform.forward * currentSpeed;
            v.y = rigidbody.velocity.y;
            rigidbody.velocity = v;
            rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void Explode()
    {
        if (!IsActive)
            return;

        IsActive = false;
        CameraController.Instance.SetTarget(null);
        SetVisualsVisibility(false);
        animator.SetTrigger("close");
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        explodeFeedback.PlayFeedbacks();
        OnExploded?.Invoke();
    }

    private void SetVisualsVisibility(bool value)
    {
        foreach (GameObject visual in visuals)
        {
            visual.SetActive(value);
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        Monster monster = collision.gameObject.GetComponentInParent<Monster>();
        if (monster != null)
        {
            monster.TakeDamage(bomb);
            Explode();
        }
    }

    private void ConvertToCar()
    {
        StartCoroutine(convertToCar());
    }

    private IEnumerator convertToCar()
    {
        isChanging = true;
        isHovering = false;
        animator.SetTrigger("close");
        rigidbody.useGravity = true;
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
        animator.SetTrigger("open");
        if(jump) 
            rigidbody.AddForce(Vector3.up*jumpForce, ForceMode.VelocityChange);

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0f, startRot.eulerAngles.y, 0f);
        float timer = 0f;
        float dur = 1f;
        //rigidbody.velocity = Vector3.zero;
        //rigidbody.useGravity = false;
        rigidbody.angularVelocity = Vector3.zero;
        Vector3 startV = rigidbody.velocity;
        while (timer<dur)
        {
            //rigidbody.velocity = new Vector3(startV.x, startV.y*(1-(timer/dur)), startV.z);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, timer/dur);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            rigidbody.angularVelocity = Vector3.zero;
        }
        
        rigidbody.useGravity = false;
        isChanging = false;

        //yield return new WaitForSeconds(999);
        
    }

    private bool isOnGround()
    {
        return getDistanceToGround() < 0.1f;
    }

    private float getDistanceToGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(pivot.position+(Vector3.up*10f), Vector3.down);
        if(Physics.Raycast(ray, out hit, 9999f, LayerMask.GetMask("Ground")))
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
    }
    
    [System.Serializable]
    class UpgradeLevel
    {
        public UpgradeType Type;
        public int Level;
    }
}
