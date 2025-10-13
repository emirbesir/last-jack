using System;
using System.Collections;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [Header("Flame Settings")]
    [SerializeField] private float flameTotalSeconds;
    [SerializeField] private float flameDecayRate;
    [SerializeField] private float minFlameIntensity;
    [Header("References")]
    [SerializeField] private Light flameLight;
    [SerializeField] private PlayerMovement playerMovement;

    // State
    private float currentFlameSeconds;
    private float startingFlameIntensity;
    private bool isFlameDepleted;
    private bool isGlowing;

    public float CurrentFlameSeconds => currentFlameSeconds;    
    public bool IsGlowing => isGlowing;

    // Events
    public event Action OnFlameDepleted;

    // Singleton
    public static Flame Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentFlameSeconds = flameTotalSeconds;
        startingFlameIntensity = flameLight.intensity;
        isFlameDepleted = false;
    }

    private void Update()
    {
        if (playerMovement.IsCrouching) return;

        DecayFlame();

        if (isGlowing) return;

        UpdateLightIntensity();
    }

    private void DecayFlame()
    {
        if (isFlameDepleted) return;
        
        currentFlameSeconds -= flameDecayRate * Time.deltaTime;

        if (currentFlameSeconds <= 0)
        {
            isFlameDepleted = true;
            OnFlameDepleted?.Invoke();
        }
    }

    private void UpdateLightIntensity()
    {
        if (isFlameDepleted) return;

        flameLight.intensity = Mathf.Clamp(GetIntensityRatio() * startingFlameIntensity, minFlameIntensity, startingFlameIntensity);
    }

    public void ReplenishFlame(float seconds)
    {
        if (isFlameDepleted) return;

        currentFlameSeconds = Mathf.Min(currentFlameSeconds + seconds, flameTotalSeconds);
    }

    public void DamageFlame(float seconds)
    {
        if (isFlameDepleted) return;

        currentFlameSeconds = Mathf.Max(currentFlameSeconds - seconds, 0);
        if (currentFlameSeconds <= 0)
        {
            isFlameDepleted = true;
            OnFlameDepleted?.Invoke();
        }
    }

    public float GetIntensityRatio()
    {
        return currentFlameSeconds / flameTotalSeconds;
    }

    public void IncreaseMaxFlame(float additionalSeconds)
    {
        flameTotalSeconds += additionalSeconds;
        currentFlameSeconds += additionalSeconds;
    }

    public void ResetFlame()
    {
        currentFlameSeconds = flameTotalSeconds;
        isFlameDepleted = false;
        UpdateLightIntensity();
    }

    public void SetGlowing(bool glowing)
    {
        isGlowing = glowing;
    }
}
