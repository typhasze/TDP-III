using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private FloatingHealthBar healthBar;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0f);
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player died!");
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BossDamage"))
        {
            TakeDamage(10f);
        }
    }
} 