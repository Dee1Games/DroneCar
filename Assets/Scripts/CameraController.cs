using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lookOffset;
    
    private RaycastHit[] hits;


    void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned! Please assign a target Transform.");
            return;
        }

        // Calculate the desired position the camera should be at
        Vector3 desiredPosition = target.position + (target.forward*offset.z) + (target.up*offset.y) + (target.right*offset.x);

        // Smoothly move the camera to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPosition;

        // Make the camera look at the car's position
        transform.LookAt( target.position + (target.up*offset.y*lookOffset));
    }
}
