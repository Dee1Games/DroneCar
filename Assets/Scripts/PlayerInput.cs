using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public float Forward;
    [HideInInspector] public float Right;
    [HideInInspector] public float Up;
    [HideInInspector] public float Brake;

    [SerializeField] private float sensivityX = 1f;
    [SerializeField] private float sensivityY = 2f;
    
    private bool isDragging = false;
    private Vector3 dragStartPos;
    private Vector3 dragPos;
    
    private void Update()
    {
        Forward = Input.GetMouseButton(0)?1f:0f;
        Brake = Input.GetMouseButton(0)?0f:1f;
        

        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }
        
        if (Input.GetMouseButton(0) && isDragging)
        {
            dragPos = Input.mousePosition;
        }

        if (!Input.GetMouseButton(0) && isDragging)
        {
            isDragging = false;
            dragStartPos = Vector3.zero;
            dragPos = Vector3.zero;
        }

        float dragX = dragPos.x - dragStartPos.x;
        Right = Mathf.Clamp((dragX / Screen.width)* sensivityX, -1f, 1f);
        
        
        float dragY = dragPos.y - dragStartPos.y;
        Up = Mathf.Clamp((dragY / Screen.height)* sensivityY, -1f, 1f);
    }
}
