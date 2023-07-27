using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        Quaternion lookRotation = Quaternion.LookRotation(PlayerVehicle.Instance.transform.forward);
        lookRotation.eulerAngles = new Vector3(transform.eulerAngles.x, lookRotation.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = lookRotation;
    }
}
