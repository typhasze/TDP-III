using UnityEngine;

public class BossTotem : MonoBehaviour
{
    private float health;
    [SerializeField] private float maxHealth = 50f;
    private BossFSM boss;
    private FloatingHealthBar healthBar;

    private void Start()
    {
        boss = FindFirstObjectByType<BossFSM>();
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        health = maxHealth;
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health <= 0)
        {
            if (boss != null)
            {
                boss.TotemDestroyed();
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerDamage"))
        {
            TakeDamage(10f); // Adjust damage value as needed
        }
    }
} 