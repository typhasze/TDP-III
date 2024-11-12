using UnityEngine;

public class BossAngerWall : MonoBehaviour
{
    [SerializeField] private int hitsToBreak = 5;
    private int currentHits = 0;
    
    private SpriteRenderer spriteRenderer;
    private Collider2D wallCollider;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        wallCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BossDamage"))
        {
            currentHits++;
            if (currentHits >= hitsToBreak)
            {
                // Find and notify the boss
                BossFSM boss = FindFirstObjectByType<BossFSM>();
                if (boss != null)
                {
                    boss.WallDestroyed();
                }
                DisableWall();
            }
        }
    }

    public void ResetWall()
    {
        currentHits = 0;
        EnableWall();
    }

    private void DisableWall()
    {
        spriteRenderer.enabled = false;
        wallCollider.enabled = false;
    }

    private void EnableWall()
    {
        spriteRenderer.enabled = true;
        wallCollider.enabled = true;
    }
}