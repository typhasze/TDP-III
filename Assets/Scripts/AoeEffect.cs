using UnityEngine;

public class AoeEffect : MonoBehaviour
{
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.2f);  // Semi-transparent red
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 0.4f);    // More opaque red
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeFrequency = 0.1f;
    
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private float shakeTimer;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.color = startColor;
        }
    }
    
    private void Update()
    {
        if (mainCamera != null)
        {
            ShakeScreen();
        }
    }
    
    private void ShakeScreen()
    {
        shakeTimer += Time.deltaTime;
        float xOffset = Mathf.Sin(shakeTimer * shakeFrequency) * shakeIntensity;
        float yOffset = Mathf.Cos(shakeTimer * shakeFrequency) * shakeIntensity;
        
        mainCamera.transform.position = originalCameraPosition + new Vector3(xOffset, yOffset, 0);
    }
    
    private void OnDestroy()
    {
        // Reset camera position when the AoE is destroyed
        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPosition;
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