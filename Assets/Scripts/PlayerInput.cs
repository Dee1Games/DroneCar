using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [HideInInspector] public float Forward;
    [HideInInspector] public float Right;
    [HideInInspector] public float Up;
    [HideInInspector] public bool Hold;
    [HideInInspector] public bool SwipeUp;

    public float JoystickX => joystick.Horizontal * sensivityJoystick;
    public float JoystickY => joystick.Vertical * sensivityJoystick;

    [SerializeField] private DynamicJoystick joystick;
    [SerializeField] private float sensivityX = 1f;
    [SerializeField] private float sensivityY = 2f;
    [SerializeField] private float sensivityJoystick = 2f;
    [SerializeField] private float swipeUpOffset = 0.8f;
    
    private bool isDragging = false;
    private Vector3 dragStartPos;
    private Vector3 dragPos;
    
    private Vector3 targetInput;
    private Vector3 lastPos;
    private bool wasHoldingLastFrame;

    private void Update()
    {
        Hold = Input.GetMouseButton(0);
        
        Forward = Input.GetMouseButton(0)?1f:0f;
        
        if (wasHoldingLastFrame)
            Right = ((Input.mousePosition.x - lastPos.x) / Screen.width) * sensivityX;
        else
            Right = 0f;
        
        if (wasHoldingLastFrame)
            Up = ((Input.mousePosition.y - lastPos.y) / Screen.height) * sensivityY;
        else
            Up = 0f;
        
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

        float dragX = ((dragPos.x - dragStartPos.x) / Screen.width);
        float dragY = ((dragPos.y - dragStartPos.y) / Screen.height);

        SwipeUp = (isDragging && dragY > swipeUpOffset);

        lastPos = Input.mousePosition;
        wasHoldingLastFrame = Input.GetMouseButton(0);
    }
}
