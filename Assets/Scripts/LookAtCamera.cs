using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool lockY = true;
    
    void Update()
    {
        if (GameManager.Instance.Player != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(GameManager.Instance.Player.transform.forward);
            if (lockY)
            {
                lookRotation.eulerAngles = new Vector3(transform.eulerAngles.x, lookRotation.eulerAngles.y, transform.eulerAngles.z);
            }
            transform.rotation = lookRotation;
        }
    }
}
