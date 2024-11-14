using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseMinScale = 0.9f;
    [SerializeField] private float pulseMaxScale = 1.1f;

    private void Update()
    {
        // Rotate the shield
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Pulse the shield size
        float pulse = Mathf.Lerp(pulseMinScale, pulseMaxScale, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
        transform.localScale = Vector3.one * pulse;
    }
} 