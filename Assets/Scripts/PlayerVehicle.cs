using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    [SerializeField] private Vector2 accel;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float rotSpeed = 10f;
    [SerializeField] private float maxHorizontalAngle = 25f;
    [SerializeField] private float maxVerticalAngle = 45f;
    [SerializeField] private Vector2 heightMinMax;
    [SerializeField] private Vector2 widthMinMax;
    [SerializeField] private float clampOffset = 0.1f;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Transform pivot;

    private float speed;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    private void Update()
    {
        float distanceToGround = getDistanceToGround();
        float targetSpeed = maxSpeed * input.Forward;
        float diff = targetSpeed - speed;
        if (diff > 0)
        {
            if (Mathf.Abs(targetSpeed - speed) > accel.y)
                speed += accel.y;
            else
                speed = targetSpeed;
        }
        else
        {
            if (Mathf.Abs(targetSpeed - speed) > accel.y)
                speed -= accel.y;
            else
                speed = targetSpeed;
        }

        input.UnlockInputRight();
        input.UnlockInputForward();
        input.UnlockInputUp();
        if (distanceToGround < heightMinMax.x+clampOffset)
            input.LockInputUp(0f, 1f);
        else if (distanceToGround > heightMinMax.y-clampOffset)
            input.LockInputUp(-1f, 0f);
        else
        {
            if (distanceToGround > heightMinMax.x && !input.Hold)
            {
                input.LockInputUp(-0.5f, -0.5f);
                input.LockInputForward(0.5f, 0.5f);
            }
        }
        if (transform.position.x < widthMinMax.x+clampOffset)
            input.LockInputRight(0f, 1f);
        else if (transform.position.x > widthMinMax.y-clampOffset)
            input.LockInputRight(-1f, 0f);
        else
            input.UnlockInputRight();
        
        
        float targetRotationAngleX = maxHorizontalAngle * input.Right;
        float targetRotationAngleY = maxVerticalAngle * input.Up;
        Quaternion targetRotation = Quaternion.Euler(targetRotationAngleY, targetRotationAngleX, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotSpeed);
    }

    private void FixedUpdate()
    {
        Vector3 direction = transform.forward * speed * Time.deltaTime;

        if ((transform.position + direction).x > widthMinMax.y || (transform.position + direction).x < widthMinMax.x)
            direction.x = 0f;
        if ((transform.position + direction).y > heightMinMax.y || (transform.position + direction).y < heightMinMax.x)
            direction.y = 0f;
        rigidbody.MovePosition(transform.position + direction);
        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, widthMinMax.x, widthMinMax.y),Mathf.Clamp(transform.position.y, heightMinMax.x, heightMinMax.y), transform.position.z);

    }

    private void LateUpdate()
    {

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
    
}
