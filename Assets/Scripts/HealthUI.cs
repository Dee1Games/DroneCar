using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    [SerializeField] private float tweenDuration = .7f;
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthFill.DOFillAmount(currentHealth / maxHealth, tweenDuration);
    }
    
    public void SetHealth(float fill)
    {
        healthFill.fillAmount = fill;
    }
}