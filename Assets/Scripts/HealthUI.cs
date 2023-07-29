using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthFill.fillAmount = (float)currentHealth / maxHealth;
    }
}