using UnityEngine;

public class Flare : MonoBehaviour
{
    private const string GROUND_ENEMY_LAYER_NAME = "Ground Enemy";
    private const string GLOW_STRENGTH_PROPERTY = "_Player_Glow_Strength";

    [Header("References")]
    [SerializeField] private Light playerLight;
    [SerializeField] private ParticleSystem pulseParticle;

    [Header("Input Settings")]
    [SerializeField] private float tapThreshold;

    [Header("Pulse (Tap) Settings")]
    [SerializeField] private float pulseFlameCost;
    [SerializeField] private float pulseRadius;
    [SerializeField] private float pulseForce;

    [Header("Glow (Hold) Settings")]
    [SerializeField] private float glowFlameCostPerSecond;
    [SerializeField] private float glowIntensity;
    [SerializeField] private float glowTransitionSpeed;
    [SerializeField] private Material flickerMaterial;

    // Private state variables
    private float holdTimer = 0.0f;
    private bool isHolding = false;
    private bool isGlowing = false;
    private float initialLightIntensity;
    private PlayerInputHandler inputHandler;
    private Flame flame;

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
        var handler = InputHandler;
        if (handler == null) return;

        handler.OnFlareStarted += HandleFlareStarted;
        handler.OnFlareCanceled += HandleFlareCanceled;
    }

    private void OnDisable()
    {
        var handler = InputHandler;
        if (handler == null) return;

        handler.OnFlareStarted -= HandleFlareStarted;
        handler.OnFlareCanceled -= HandleFlareCanceled;
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

        var flameComponent = FlameComponent;
        if (flameComponent == null)
        {
            return;
        }

        if (shouldGlow && flameComponent.CurrentFlameSeconds > glowCostThisFrame)
        {
            isGlowing = true;
            flameComponent.DamageFlame(glowCostThisFrame);
            flameComponent.SetGlowing(true);
            flickerMaterial.SetFloat(GLOW_STRENGTH_PROPERTY, 1f);

            playerLight.intensity = Mathf.Lerp(playerLight.intensity, glowIntensity, Time.deltaTime * glowTransitionSpeed);
        }
        else
        {
            isGlowing = false;
            float intensityRatio = flameComponent.GetIntensityRatio();

            playerLight.intensity = Mathf.Lerp(playerLight.intensity, initialLightIntensity * intensityRatio, Time.deltaTime * glowTransitionSpeed * 1.5f);
        }

        if (Mathf.Abs(playerLight.intensity - initialLightIntensity) < 0.1f && !isGlowing)
        {
            flameComponent.SetGlowing(false);
            flickerMaterial.SetFloat(GLOW_STRENGTH_PROPERTY, 0f);
            playerLight.intensity = initialLightIntensity * flameComponent.GetIntensityRatio();
        }
    }

    private void ExecutePulse()
    {
        var flameComponent = FlameComponent;
        if (flameComponent == null || flameComponent.CurrentFlameSeconds < pulseFlameCost) return;

        flameComponent.DamageFlame(pulseFlameCost);

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

    private PlayerInputHandler InputHandler
    {
        get
        {
            if (inputHandler == null)
            {
                inputHandler = PlayerInputHandler.Instance;
            }

            return inputHandler;
        }
    }

    private Flame FlameComponent
    {
        get
        {
            if (flame == null)
            {
                flame = Flame.Instance;
            }

            return flame;
        }
    }
}
