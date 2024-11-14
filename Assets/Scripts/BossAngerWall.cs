using UnityEngine;

public class BossAngerWall : MonoBehaviour
{
    [SerializeField] private int hitsToBreak = 5;
    [SerializeField] private Color startColor = new Color(0.25f, 0.17f, 0.11f); // #412B1D
    [SerializeField] private Color endColor = new Color(0.8f, 0.4f, 0.2f);      // More intense orange/brown
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
            float damageProgress = (float)currentHits / hitsToBreak;
            spriteRenderer.color = Color.Lerp(startColor, endColor, damageProgress);
            
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
        spriteRenderer.color = startColor;
        EnableWall();
    }

    private void DisableWall()
    {
        spriteRenderer.enabled = false;
        wallCollider.enabled = false;
        
        // Add screen shake when wall breaks
        if (Camera.main.TryGetComponent<CameraShake>(out var cameraShake))
        {
            cameraShake.ShakeCamera(0.3f, 0.2f); // Duration: 0.3s, Intensity: 0.2
        }
    }

    private void EnableWall()
    {
        spriteRenderer.enabled = true;
        wallCollider.enabled = true;
    }
}