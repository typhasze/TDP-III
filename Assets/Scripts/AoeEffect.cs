using UnityEngine;

public class AoeEffect : MonoBehaviour
{
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.2f);  // Semi-transparent red
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 0.4f);    // More opaque red
    
    private SpriteRenderer spriteRenderer;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = startColor;
        }
    }
    
    public void UpdateColor(float progress)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(startColor, endColor, progress);
        }
    }
} 