using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //slider.transform.rotation = Quaternion.Inverse(transform.parent.rotation);
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        slider.value = currentHealth / maxHealth;
    }
}
