using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawBlade : MonoBehaviour
{
    public float DPS = 10f;
    [SerializeField] private ParticleSystem bladeEffect;

    private CarCore core;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameManager.CPlayer))
        {
            if (other.TryGetComponent(out CarCore carCore))
            {
                core = carCore;
                bladeEffect.Play();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameManager.CPlayer))
        {
            if (other.TryGetComponent(out CarCore carCore))
            {
                bladeEffect.Stop();
                core = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (core)
        {
            
            core.TakeDamage(other.transform.position, DPS);
        }
    }
}
