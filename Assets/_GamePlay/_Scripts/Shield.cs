using DG.Tweening;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public Transform trackTransform;
    
    private Collider _collider;
    
    public float activePeriod;

    private Tween tween;

    public Vector3 activeScale;
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

    public void Activate(Transform _track, Giant_Core core)
    {
        trackTransform = _track;
        tween = DOVirtual.DelayedCall(activePeriod, () =>
        {
            _collider.enabled = !_collider.enabled;
            if (_collider.enabled && !core.IsDead)
            {
                transform.DOScale(activeScale, fadeInDuration).SetEase(ease);
                UI_Core._.shieldIcon.DOFillAmount(1, fadeInDuration).SetEase(ease);
            }
            else
            {
                transform.DOScale(Vector3.zero, fadeOutDuration).SetEase(ease);
                UI_Core._.shieldIcon.DOFillAmount(0, fadeOutDuration).SetEase(ease);
            }
        }).SetLoops(-1);
    }

    public void Deactivate()
    {
        if (tween != null)
        {
            tween.Kill();
        }
    }
}
