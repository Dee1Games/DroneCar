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
            ConvertToDrone();
        
        if (isHovering && height < convertHeight)
            ConvertToCar();
        
        if(!isHovering && input.SwipeUp)
            ConvertToDrone();
        
        if (isHovering)
        {
            float speedDiff = (droneMaxSpeed * input.Forward) - currentSpeed;
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

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
            direction = new Vector3(input.Right, 0f, input.Forward);
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
        }
        else
        {
            Vector3 v = transform.forward * currentSpeed;
            v.y = rigidbody.velocity.y;
            rigidbody.velocity = v;
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
    
    private void ConvertToDrone()
    {
        StartCoroutine(convertToDrone());
    }
    
    private IEnumerator convertToDrone()
    {
        isChanging = true;
        isHovering = true;
        animator.SetTrigger("open");
        rigidbody.AddForce(transform.up*jumpForce, ForceMode.VelocityChange);
        while (rigidbody.velocity.y>-0.1f)
        {
            yield return new WaitForFixedUpdate();
        }
        rigidbody.useGravity = false;
        isChanging = false;
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
