using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float lookOffset;
    [SerializeField] private float longShotDuration;
    [SerializeField] private float longShotOffset;
    [SerializeField] private float longShotRadius;
    [SerializeField] private float longShotHeight;
    private MeshRenderer[] propRenderers;

    
    private Transform target;
    private RaycastHit[] hits;
    private bool following;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void FixedUpdate()
    {
        if (target == null || !following)
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
        
        //HideCameraOverlaps();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        following = true;
        Reset();
    }

    public void Reset()
    {
        if (target == null)
            return;
        
        transform.position = target.position + (target.forward*offset.z) + (target.up*offset.y) + (target.right*offset.x);
        transform.LookAt( target.position + (target.up*offset.y*lookOffset));
    }

    public void TakeLongShot(Vector3 hitPoint, Vector3 axis)
    {
        if (!following)
            return;
        
        StartCoroutine(takeLongShot(hitPoint, axis));
    }

    private IEnumerator takeLongShot(Vector3 hitPoint, Vector3 axis)
    {
        //Vector3 axis = (hitPoint - GameManager.Instance.Monster.transform.position).normalized;
        //axis.y = 0f;
        following = false;
        float timer = 0f;
        // Vector3 startPos = transform.position;
        // Vector3 endPos = hitPoint - (axis*longShotOffset);
        // while (timer<longShotDuration)
        // {
        //     Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(endPos), timer / longShotDuration);
        //     transform.position = Vector3.Lerp(transform.position, endPos, timer / longShotDuration);
        //     timer += Time.deltaTime;
        //     yield return new WaitForEndOfFrame();
        // }

        // Camera Long shot Fixed
        Vector3 monsterPos = GameManager.Instance.Monster.transform.position;
        Vector3 carPos = transform.position;
        Vector3 carDir = (carPos - monsterPos).normalized;
        if (carDir.x > 0)
            carDir.x = 1f;
        else if (carDir.x < 0)
            carDir.x = -1f;
        if (carDir.z > 0)
            carDir.z = 1f;
        else if (carDir.z < 0)
            carDir.z = -1f;
        carDir.y = 0f;
        Vector3 endPos = monsterPos + (carDir * longShotRadius);
        endPos.y = longShotHeight;
        
        
        //var dir = CarCore._.transform.forward.normalized;
        //dir.y = 0f;
        var basePos = transform.position;
        //var endPos = transform.position - (dir * longShotOffset);
        var baseRot = transform.rotation;
        while (timer <= longShotDuration)
        {
            transform.position = Vector3.Lerp(basePos, endPos, timer / longShotDuration);
            Vector3 dir = (GameManager.Instance.Monster.com.position - transform.position).normalized;
            transform.rotation = Quaternion.Lerp(baseRot, Quaternion.LookRotation(dir, Vector3.up), timer / longShotDuration);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if (GameManager.Instance.Monster.IsDead)
        {
            UIManager.Instance.ShowScreen(UIScreenID.EndLevel);
        }
        else
        {
            UIManager.Instance.ShowScreen(UIScreenID.EndRun);
        }
        
        // if (GameManager.Instance.Monster.IsDead)
        // {
        //     UserManager.Instance.NextLevel();
        //     LevelManager.Instance.InitCurrentLevel();
        //     MergePlatform.Instance.ClearPlatform();
        // }
        // GameManager.Instance.GoToUpgradeMode();
    }

    private void HideCameraOverlaps()
    {
        if (propRenderers != null)
        {
            foreach (MeshRenderer renderer in propRenderers)
            {
                renderer.enabled = true;
            }
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
