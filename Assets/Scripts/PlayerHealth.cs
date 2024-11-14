using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damageTaken = 10f;
    private float currentHealth;
    private FloatingHealthBar healthBar;
    private bool isImmortal = false;
    [SerializeField] private GameObject shieldEffectPrefab;
    private GameObject activeShieldEffect;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    public void TakeDamage(float damage)
    {
        if (isImmortal) return;

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
            TakeDamage(damageTaken);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1f);
        }
    }

    public void GrantImmortality()
    {
        isImmortal = true;
        SpawnShieldEffect();
        Debug.Log("Player is now immortal!");
    }

    private void SpawnShieldEffect()
    {
        if (shieldEffectPrefab != null && activeShieldEffect == null)
        {
            activeShieldEffect = Instantiate(shieldEffectPrefab, transform);
            activeShieldEffect.transform.localPosition = Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        if (activeShieldEffect != null)
        {
            Destroy(activeShieldEffect);
        }
    }
} 