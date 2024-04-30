using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private ParticleSystem explosion;

    public void Init()
    {
        content.SetActive(true);
        explosion.gameObject.SetActive(false);
    }
    
    public void Explode()
    {
        content.SetActive(false);
        explosion.gameObject.SetActive(true);
        explosion.Play();
    }
}
