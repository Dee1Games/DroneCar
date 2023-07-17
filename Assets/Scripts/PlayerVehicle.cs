using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float acceleration = 500f;
    [SerializeField] private float brakingForce = 300f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxHorizontalAngle = 15f;
    [SerializeField] private float maxVerticalAngle = 45f;

    [Header("References")]
    [SerializeField] private PlayerInput input;
    
    [Header("Wheels")]
    [SerializeField] private WheelCollider[] movingWheelColliders;
    [SerializeField] private WheelCollider[] turningWheelColliders;
    [SerializeField] private WheelCollider[] allWheelColliders;
    [SerializeField] private Transform[] allWheelMeshes;

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentTurnAngle = 0f;
    
    
    public float pitchSpeed = 2f;
    public float rollSpeed = 3f;

    private float rotationSmoothness = 0.2f;

    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        currentAcceleration = acceleration * input.Forward;
        currentBrakeForce = brakingForce * input.Brake;
        currentTurnAngle = maxHorizontalAngle * input.Right;
        
        // foreach (WheelCollider movingWheelCollider in movingWheelColliders)
        // {
        //     movingWheelCollider.motorTorque = currentAcceleration;
        // }
        //
        // foreach (WheelCollider wheelCollider in allWheelColliders)
        // {
        //     wheelCollider.brakeTorque = currentBrakeForce;
        // }
        //
        // foreach (WheelCollider turningWheelCollider in turningWheelColliders)
        // {
        //     turningWheelCollider.steerAngle = currentTurnAngle;
        // }
        //
        for (int i = 0 ; i < allWheelMeshes.Length ; i++)
        {
            updateWheelVisuals(allWheelColliders[i], allWheelMeshes[i]);
        }

        bool isOnGround = this.isOnGround();
        if (!isOnGround)
        {
            if (input.Forward < 0.1f)
            {
                input.Up = -0.5f;
                input.Forward = 0.5f;
            }
        }
        else
        {
            if (input.Up > 0.1f)
            {
                rigid.useGravity = false;
            }
            else
            {
                rigid.useGravity = true;
            }
        }

        //rigid.useGravity = (isOnGround() && input.Up > 0.1f) || ;

        // Calculate movement direction
        //Vector3 movement = new Vector3(input.Right, input.Up, input.Forward) * speed * Time.deltaTime;
        //transform.Translate(movement);

        Vector3 direction = new Vector3(input.Right, input.Up, input.Forward);
        Debug.LogError(direction);
        //rigid.AddForce(direction * airForce);
        rigid.velocity = (direction * speed);

        // Calculate rotation
        float roll = input.Right * rollSpeed;
        float pitch = -input.Up * pitchSpeed;
        Quaternion targetRotation = Quaternion.Euler(pitch, roll, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness);

    }

    private void updateWheelVisuals(WheelCollider wheelCollider, Transform wheelMesh)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelMesh.position = position;
        wheelMesh.rotation = rotation;
    }

    private bool isOnGround()
    {
        return getDistanceToGround() < 0.1f;
    }

    private float getDistanceToGround()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position+(Vector3.up*1f), Vector3.down);
        if(Physics.Raycast(ray, out hit, 9999f, LayerMask.GetMask("Ground")))
        {
            return Mathf.Abs(hit.distance-1f);
        }

        return 9999f;
    }
}
