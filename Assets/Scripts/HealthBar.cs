using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthBar;

    public void UpdateHeath(float currentHealth, float totalHealth)
    {
        healthBar.fillAmount = currentHealth / totalHealth;
    }
}
