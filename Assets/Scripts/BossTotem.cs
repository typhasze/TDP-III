using UnityEngine;

public class BossTotem : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    private BossFSM boss;

    private void Start()
    {
        boss = FindFirstObjectByType<BossFSM>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
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