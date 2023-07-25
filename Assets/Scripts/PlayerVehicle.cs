using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    [Header("Car Properties")]
    [SerializeField] private float carAcceleration = 500f;
    [SerializeField] private float carReverseAcceleration = 500f;
    [SerializeField] private float carMaxSpeed = 100f;
    
    [Header("Drone Properties")]
    [SerializeField] private float droneAcceleration;
    [SerializeField] private float droneReverseAcceleration;
    [SerializeField] private float jumpForce;
    [SerializeField] private float droneMaxSpeed;
    
    [Header("Other Properties")]
    [SerializeField] private float convertHeight = 2f;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Transform pivot;
    
    
    private bool isHovering;
    private bool isChanging;
    
    private float currentSpeed;

    private Rigidbody rigidbody;
    private Animator animator;
    private Vector3 direction;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isHovering = false;
        isChanging = false;
    }

    private void Update()
    {
        if (isChanging)
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
            float speedDiff = (droneMaxSpeed * inputForward) - currentSpeed;
            if (speedDiff > 0)
            {
                float delta = droneAcceleration * Time.deltaTime;
                if (delta + currentSpeed <= droneMaxSpeed)
                    currentSpeed += delta;
                else
                    currentSpeed = droneMaxSpeed;
            }
            else {
                float delta = -droneReverseAcceleration * Time.deltaTime;
                if (delta + currentSpeed >= 0f)
                    currentSpeed += delta;
                else
                    currentSpeed = 0f;
            }
            
            if (input.Hold)
            {
                transform.Rotate(Vector3.up, input.Right);
                transform.Rotate(Vector3.right, -input.Up);
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
            float speedDiff = (carMaxSpeed * input.Forward) - currentSpeed;
            if (speedDiff > 0)
            {
                float delta = carAcceleration * Time.deltaTime;
                if (delta + currentSpeed <= carMaxSpeed)
                    currentSpeed += delta;
                else
                    currentSpeed = carMaxSpeed;
            }
            else {
                float delta = -carReverseAcceleration * Time.deltaTime;
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
            transform.Rotate(Vector3.up, input.JoystickX);
        }
    }

    private void FixedUpdate()
    {
        if (isChanging)
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
    
}
