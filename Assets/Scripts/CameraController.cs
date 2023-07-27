using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lookOffset;
    [SerializeField] private MeshRenderer[] propRenderers;

    
    private Transform target;
    private RaycastHit[] hits;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position the camera should be at
        Vector3 desiredPosition = target.position + (target.forward*offset.z) + (target.up*offset.y) + (target.right*offset.x);

        // Smoothly move the camera to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPosition;

        // Make the camera look at the car's position
        transform.LookAt( target.position + (target.up*offset.y*lookOffset));
    }
    
    private void LateUpdate()
    {
        if (target == null)
            return;
        
        HideCameraOverlaps();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        Reset();
    }

    public void Reset()
    {
        if (target == null)
            return;
        
        transform.position = target.position + (target.forward*offset.z) + (target.up*offset.y) + (target.right*offset.x);
        transform.LookAt( target.position + (target.up*offset.y*lookOffset));
    }

    private void HideCameraOverlaps()
    {
        foreach (MeshRenderer renderer in propRenderers)
        {
            renderer.enabled = true;
        }
        
        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);
        hits = Physics.RaycastAll(transform.position, direction, distance, LayerMask.GetMask("Props"));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null)
            {
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
        }

        Collider[] camColliders = Physics.OverlapSphere(transform.position, 2f, LayerMask.GetMask("Props"));
        foreach (Collider collider in camColliders)
        {
            Renderer renderer = collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
    }
}
