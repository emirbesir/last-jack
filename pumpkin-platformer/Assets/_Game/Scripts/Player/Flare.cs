using UnityEngine;

public class Flare : MonoBehaviour
{
    private const string GROUND_ENEMY_LAYER_NAME = "Ground Enemy";

    [Header("References")]
    public Light playerLight;
    public ParticleSystem pulseParticle;

    [Header("Input Settings")]
    public float tapThreshold;

    [Header("Pulse (Tap) Settings")]
    public float pulseFlameCost;
    public float pulseRadius;
    public float pulseForce;

    [Header("Glow (Hold) Settings")]
    public float glowFlameCostPerSecond;
    public float glowIntensity;
    public float glowTransitionSpeed;

    // Private state variables
    private float holdTimer = 0.0f;
    private bool isHolding = false;
    private bool isGlowing = false;
    private float initialLightIntensity;

    // Public property for enemies to check
    public bool IsGlowing => isGlowing;

    void Start()
    {
        if (playerLight != null)
        {
            initialLightIntensity = playerLight.intensity;
        }
    }

    private void OnEnable()
    {
        PlayerInputHandler.Instance.OnFlareStarted += HandleFlareStarted;
        PlayerInputHandler.Instance.OnFlareCanceled += HandleFlareCanceled;
    }

    private void OnDisable()
    {
        PlayerInputHandler.Instance.OnFlareStarted -= HandleFlareStarted;
        PlayerInputHandler.Instance.OnFlareCanceled -= HandleFlareCanceled;
    }

    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
        }
        HandleGlow();
    }

    private void HandleFlareStarted()
    {
        isHolding = true;
        holdTimer = 0.0f;
    }

    private void HandleFlareCanceled()
    {
        if (isHolding && holdTimer < tapThreshold)
        {
            ExecutePulse();
        }
        isHolding = false;
    }

    private void HandleGlow()
    {
        bool shouldGlow = isHolding && holdTimer >= tapThreshold;
        float glowCostThisFrame = glowFlameCostPerSecond * Time.deltaTime;

        if (shouldGlow && Flame.Instance.CurrentFlameSeconds > glowCostThisFrame)
        {
            isGlowing = true;
            Flame.Instance.DamageFlame(glowCostThisFrame);
            Flame.Instance.SetGlowing(true);

            playerLight.intensity = Mathf.Lerp(playerLight.intensity, glowIntensity, Time.deltaTime * glowTransitionSpeed);
        }
        else
        {
            isGlowing = false;
            float intensityRatio = Flame.Instance.GetIntensityRatio();

            playerLight.intensity = Mathf.Lerp(playerLight.intensity, initialLightIntensity * intensityRatio, Time.deltaTime * glowTransitionSpeed);
        }

        if (Mathf.Abs(playerLight.intensity - initialLightIntensity) < 0.01f && !isGlowing)
        {
            playerLight.intensity = initialLightIntensity * Flame.Instance.GetIntensityRatio();
            Flame.Instance.SetGlowing(false);
        }
    }

    private void ExecutePulse()
    {
        if (Flame.Instance.CurrentFlameSeconds < pulseFlameCost) return;

        Flame.Instance.DamageFlame(pulseFlameCost);

        if (pulseParticle != null) pulseParticle.Play();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pulseRadius, LayerMask.GetMask(GROUND_ENEMY_LAYER_NAME));
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.AddExplosionForce(pulseForce, transform.position, pulseRadius);   
            }
        }
    }
}
