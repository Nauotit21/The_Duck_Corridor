// Flashlight.cs
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Références")]
    public Light spotLight;
    public DarknessZone darknessZone;

    [Header("Paramètres lampe")]
    public float intensity     = 3.5f;
    public float range         = 8.0f;
    public float spotAngle     = 35.0f;
    public Color lightColor    = new Color(0.9f, 0.85f, 0.7f);

    [Header("Flickering (optionnel)")]
    public bool  enableFlicker = true;
    public float flickerSpeed  = 0.08f;
    [Range(0f, 1f)] public float flickerAmount = 0.1f;

    private float _baseIntensity;
    private float _flickerTimer;

    void Start()
    {
        spotLight.type      = LightType.Spot;
        spotLight.intensity = intensity;
        spotLight.range     = range;
        spotLight.spotAngle = spotAngle;
        spotLight.color     = lightColor;
        spotLight.shadows   = LightShadows.Soft;

        _baseIntensity = intensity;
    }

    void Update()
    {
        HandleFlicker();
        BoostDarknessInLightDirection();
    }

    void HandleFlicker()
    {
        if (!enableFlicker) return;

        _flickerTimer -= Time.deltaTime;
        if (_flickerTimer <= 0f)
        {
            float noise = Random.Range(1f - flickerAmount, 1f);
            spotLight.intensity = _baseIntensity * noise;
            _flickerTimer = flickerSpeed + Random.Range(0f, 0.05f);
        }
    }

    // La lampe "repousse" légèrement la sphère d'ombre dans sa direction
    void BoostDarknessInLightDirection()
    {
        if (darknessZone == null) return;
        // Simple : on augmente légèrement le innerRadius quand la lampe est allumée
        // Pour un effet directionnel avancé, il faudrait un shader avec le vecteur lampe
        darknessZone.innerRadius = spotLight.enabled
            ? darknessZone.innerRadius  // géré dans le shader
            : 1.5f;
    }

    // Appelé par un bouton VR / Input
    public void Toggle() => spotLight.enabled = !spotLight.enabled;
}