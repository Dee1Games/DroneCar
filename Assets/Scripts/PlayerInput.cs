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
    
    private Vector3 targetInput;
    private Vector3 lastPos;

    private void Update()
    {
        Hold = Input.GetMouseButton(0);
        Forward = Input.GetMouseButton(0)?1f:0f;
        Right = ((Input.mousePosition.x - lastPos.x) / Screen.width) * sensivityX;
        Up = ((Input.mousePosition.y - lastPos.y) / Screen.height) * sensivityY;
        SwipeUp = joystick.Vertical > swipeUpOffset;

        lastPos = Input.mousePosition;
    }
}
