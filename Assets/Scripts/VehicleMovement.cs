using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float force;
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddForce(transform.forward*force, ForceMode.Acceleration);
        }
    }
}
