using DG.Tweening;
using UnityEngine;

public class Shield : MonoBehaviour, IHitable
{
    public Transform trackTransform;
    public float health = 100;
    public float maxHealth = 100;
    
    private Collider _collider;
    
    public float activePeriod;

    private Tween tween;

    public Vector3 activeScale = Vector3.one;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;

    public Ease ease;
    void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void Update()
    {
        if (trackTransform)
        {
            transform.position = trackTransform.position;
        }
    }

    public void SetTarget(Transform target)
    {
        trackTransform = target;
    }
    public void Activate()
    {
        if (!enabled) return;

        _collider.enabled = true;
        transform.position = trackTransform.position;
        transform.DOScale(activeScale, fadeInDuration).SetEase(ease);
//        UI_Core._.shieldIcon.DOFillAmount(1, fadeInDuration).SetEase(ease);
    }

    public void Deactivate()
    {
        if (!enabled) return;

        _collider.enabled = false;
        transform.DOScale(Vector3.zero, fadeOutDuration).SetEase(ease);
     //   UI_Core._.shieldIcon.DOFillAmount(0, fadeOutDuration).SetEase(ease);
    }

    public void OnHit(CarCore core, float damage)
    {
        
    }
}
