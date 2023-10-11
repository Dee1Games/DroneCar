using DG.Tweening;
using RaycastPro.Detectors;
using RaycastPro.RaySensors;
using UnityEngine;

public class Dish : Packable
{
    public TargetRay targetRay;
    public Transform target;

    public SightDetector sightDetector;

    public ParticleSystem effect;

    private Sequence _sequence;
    
    
}
