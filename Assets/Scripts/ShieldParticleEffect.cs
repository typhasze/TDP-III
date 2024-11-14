using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ShieldParticleEffect : MonoBehaviour
{
    private ParticleSystem shieldParticles;
    
    private void Awake()
    {
        shieldParticles = GetComponent<ParticleSystem>();
        SetupParticleSystem();
    }

    private void SetupParticleSystem()
    {
        var main = shieldParticles.main;
        main.loop = true;
        main.startLifetime = 1f;
        main.startSpeed = 0f;
        main.startSize = 0.2f;
        main.startColor = new Color(0.7f, 0.9f, 1f, 0.4f);

        var emission = shieldParticles.emission;
        emission.rateOverTime = 20f;

        var shape = shieldParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 1f;
        shape.radiusThickness = 1f;

        var colorOverLifetime = shieldParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { 
                new GradientColorKey(new Color(0.7f, 0.9f, 1f), 0.0f),
                new GradientColorKey(new Color(0.7f, 0.9f, 1f), 1.0f) 
            },
            new GradientAlphaKey[] { 
                new GradientAlphaKey(0f, 0.0f),
                new GradientAlphaKey(0.4f, 0.2f),
                new GradientAlphaKey(0.4f, 0.8f),
                new GradientAlphaKey(0f, 1.0f)
            }
        );
        colorOverLifetime.color = gradient;
    }
} 