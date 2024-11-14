using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void ShakeCamera(float duration, float intensity)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    private IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }
} 