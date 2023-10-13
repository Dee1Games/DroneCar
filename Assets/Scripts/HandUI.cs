using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float delta;
    [SerializeField] private float delay = 1f;
    [SerializeField] private Vector3 offset;
    
    private Vector3 A;
    private Vector3 B;
    private float lastTimeStoped;


    public void Init(Vector3 a, Vector3 b)
    {
        lastTimeStoped = -999f;
        A = a + offset;
        B = b + offset;

        transform.position = a;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            transform.localScale = Vector3.zero;
        } else
        {
            if (Vector3.Distance(transform.position, B) < delta)
            {
                if(lastTimeStoped<0f)
                    lastTimeStoped = Time.timeSinceLevelLoad;
                if (Time.timeSinceLevelLoad - lastTimeStoped > delay)
                {
                    Init(A, B);
                }
            }
            else
            {
                transform.localScale = Vector3.one;
                transform.Translate((B-transform.position).normalized*speed*Time.deltaTime);
            }
        }
    }
}
