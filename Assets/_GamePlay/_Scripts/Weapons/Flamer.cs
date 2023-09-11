using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamer : MonoBehaviour
{
    public bool stop;
    
    public float periodTimer;
    public float flameTime;
    public float currentTime;
    
    public ParticleSystem flamer;
    void Update()
    {
        currentTime += Time.deltaTime;
        if (!stop)
        {
            
        }
        flamer.Play();
    }
}
