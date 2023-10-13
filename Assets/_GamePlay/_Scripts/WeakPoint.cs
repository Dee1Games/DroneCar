using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WeakPoint : MonoBehaviour, IHitable
{
    public Giant_Core myCore;

    public static WeakPoint CurrentActive;
    
    public Target target;
    
    public int index;
    public int Number
    {
        set
        {
            index = value;
            if (textNumber)
            {
                textNumber.text = index.ToString();
            }
        }
    }
    
    private static int currentIndex;
    public static int CurrentIndex
    {
        get => currentIndex;
        set
        {
            currentIndex = value;
            CurrentActive = Monster._.weakPoints.First(w => w.index == value);
            CurrentActive.transform.DOScale(.4f, 1f);
            CurrentActive.target.enabled = true;
            UI_Core._.weakPointIndex.text = value.ToString();
        }
    }
    
    [Title("General")] public TextMeshPro textNumber;
    
    public float damagePlus;
    public float damageMultiplier = 1f;

    [Title("Art")]
    private Explodable explodable;
    public Limb attachedLimb;
    public ParticleSystem explodeEffect;

    [Title("Particle", "The random effect remains constant.")]
    public ParticleSystem[] particle;
    
    public void Init(int _index)
    {
        gameObject.SetActive(true);
        index = _index;
        if (index == 1)
        {
            CurrentActive = this;
        }
        Number = _index;
    }
    
    private void Awake()
    {
        if (!myCore) myCore = GetComponentInParent<Giant_Core>();
        explodable = GetComponentInChildren<Explodable>();
        textNumber = GetComponentInChildren<TextMeshPro>();
    }
    public void FindCore()
    {
        myCore = GetComponentInParent<Giant_Core>();
    }

    private void OnValidate()
    {
        Number = index;
    }

    public void OnHit(CarCore core, float damage = 10f)
    {
        if (currentIndex != index) return;
        
       CurrentIndex = index + 1;

        myCore.TakeDamage(damage * damageMultiplier + damagePlus);
        if (explodable)
        {
            explodable.transform.SetParent(null);
            explodable.Explode(transform.position);
        }
        if (explodeEffect)
        {
            var effect = Instantiate(explodeEffect);
            effect.transform.position = core.transform.position;
            effect.transform.up = -core.transform.forward;
        }
        attachedLimb?.Dismember();
        
        transform.DOScale(0, 1f).OnComplete(() => gameObject.SetActive(false));
    }
}
