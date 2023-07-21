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
    
    [SerializeField] private Vector3 lerpSpeed;
    [SerializeField] private float sensivityX = 1f;
    [SerializeField] private float sensivityY = 2f;
    [SerializeField] private Vector2 deadZoneX;
    [SerializeField] private Vector2 deadZoneY;

    
    private bool isDragging = false;
    private Vector3 dragStartPos;
    private Vector3 dragPos;
    private Vector3 targetInput;

    private Vector2 forwardInputBounds;
    private Vector2 rightInputBounds;
    private Vector2 upInputBounds;

    private void Start()
    {
        forwardInputBounds = new Vector2(-1f, 1f);
        rightInputBounds = new Vector2(-1f, 1f);
        upInputBounds = new Vector2(-1f, 1f);
    }
    
    private void Update()
    {
        Hold = Input.GetMouseButton(0);
        targetInput.z = Mathf.Clamp(Input.GetMouseButton(0)?1f:0f, forwardInputBounds.x, forwardInputBounds.y);

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

        float dragX = ((dragPos.x - dragStartPos.x) / Screen.width) * sensivityX;
        if (dragX >= deadZoneX.y || dragX <= deadZoneX.x)
            targetInput.x = Mathf.Clamp(dragX, rightInputBounds.x, rightInputBounds.y);
        else
            targetInput.x = 0f;
        
        
        float dragY = ((dragPos.y - dragStartPos.y) / Screen.height) * sensivityY;
        if (dragY >= deadZoneY.y || dragY <= deadZoneY.x)
            targetInput.y = -Mathf.Clamp(dragY, upInputBounds.x, upInputBounds.y);
        else
            targetInput.y = 0f;


        /*if (Mathf.Abs(targetInput.z - Forward) > lerpSpeed.z)
            Forward += lerpSpeed.z * Mathf.Sign(targetInput.z - Forward);
        else
            Forward = targetInput.z;
        
        if (Mathf.Abs(targetInput.x - Right) > lerpSpeed.x)
            Right += lerpSpeed.x * Mathf.Sign(targetInput.x - Right);
        else
            Right = targetInput.x;
        
        if (Mathf.Abs(targetInput.y - Up) > lerpSpeed.y)
            Up += lerpSpeed.y * Mathf.Sign(targetInput.y - Up);
        else
            Up = targetInput.y;*/
        
        Forward = Mathf.Lerp(Forward, targetInput.z, lerpSpeed.z);
        Right = Mathf.Lerp(Right, targetInput.x, lerpSpeed.x);
        Up = Mathf.Lerp(Up, targetInput.y, lerpSpeed.y);
    }

    public void LockInputUp(float min , float max)
    {
        upInputBounds = new Vector2(min, max);
    }
    
    public void UnlockInputUp()
    {
        upInputBounds = new Vector2(-1f, 1f);
    }
    
    public void LockInputRight(float min , float max)
    {
        rightInputBounds = new Vector2(min, max);
    }
    
    public void UnlockInputRight()
    {
        rightInputBounds = new Vector2(-1f, 1f);
    }
    
    public void LockInputForward(float min , float max)
    {
        forwardInputBounds = new Vector2(min, max);
    }
    
    public void UnlockInputForward()
    {
        forwardInputBounds = new Vector2(-1f, 1f);
    }
}
