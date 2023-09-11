using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Track : MonoBehaviour
{
    public Image alertImage;

    public float fadeIn = .6f;
    public float fadeOut = .6f;

    public Ease easeType = Ease.OutElastic;

    private void Awake()
    {
        alertImage = GetComponent<Image>();
        _ = this;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private Tweener alertTween;
    public void DoAlert(float duration)
    {
        alertTween = alertImage.DOFillAmount(1, duration).OnComplete(() =>
        {
            alertImage.DOColor(Color.red, .4f);
            alertImage.transform.DOPunchScale(Vector3.one * 0.5f, .4f);
        });
    }
    public void ResetAlert(float duration)
    {
        if (alertTween != null && alertTween.IsPlaying())
        {
            alertTween.Kill();
        }
        alertTween = alertImage.DOFillAmount(0, duration).OnComplete(() =>
        {
            alertImage.transform.DOPunchScale(Vector3.one * 0.5f, .4f);
            alertImage.DOColor(Color.white, .4f);
        });
    }
    public float turnSpeed = 30f;
    public void Update()
    {
        if (freeRotate)
        {
            rotateTransform.Rotate(0,0,turnSpeed * Time.deltaTime);
        }
    }

    public Transform rotateTransform;
    private bool freeRotate;

    private Tweener trackTween;
    public void Begin()
    {
        if (trackTween != null && trackTween.IsPlaying())
        {
            trackTween.Kill();
        }
        Debug.Log("Track Begins");
        alertImage.fillAmount = 0;
        gameObject.SetActive(true);
        trackTween = transform.DOScale(Vector3.one, fadeIn).SetEase(easeType).OnComplete(() =>
        {
            freeRotate = true;
        });
    }

    public void End()
    {
        if (trackTween != null && trackTween.IsPlaying())
        {
            trackTween.Kill();
        }

        alertImage.fillAmount = 0;
        trackTween = transform.DOScale(Vector3.zero, fadeOut).SetEase(easeType).OnComplete(() =>
        {
            gameObject.SetActive(false);
            freeRotate = false;
        });
    }
}
