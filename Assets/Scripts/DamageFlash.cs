using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.3f); // Red with 30% opacity

    private Image flashImage;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        flashImage = GetComponent<Image>();
        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        // Set the starting color
        flashImage.color = flashColor;

        // Fade out
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            float alpha = Mathf.Lerp(flashColor.a, 0f, elapsedTime / flashDuration);
            flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we end at zero alpha
        flashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
        flashCoroutine = null;
    }
} 