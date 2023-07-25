using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerVehicle playerVechicle;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MeshRenderer[] buildingsRenderers;
    
    private RaycastHit[] cameraHits;

    private void LateUpdate()
    {
        HideCameraOverlaps();
    }

    private void HideCameraOverlaps()
    {
        foreach (MeshRenderer renderer in buildingsRenderers)
        {
            renderer.enabled = true;
        }
        
        // Raycast from camera to target and get the hits
        Vector3 direction = (playerVechicle.transform.position - cameraController.transform.position).normalized;
        float distance = Vector3.Distance(cameraController.transform.position, playerVechicle.transform.position);
        cameraHits = Physics.RaycastAll(cameraController.transform.position, direction, distance, LayerMask.GetMask("Props"));
        foreach (RaycastHit hit in cameraHits)
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

        Collider[] camColliders = Physics.OverlapSphere(cameraController.transform.position, 2f, LayerMask.GetMask("Props"));
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
