using UnityEngine;
using UnityEngine.UI;

public class FloatingShieldBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;

    public void UpdateShieldBar(float currentShield, float maxShield)
    {
        slider.value = currentShield / maxShield;
    }
} 