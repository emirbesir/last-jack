using System;
using UnityEngine;

public class Flame : MonoBehaviour
{
    [Header("Flame Settings")]
    [SerializeField] private float flameTotal = 120f;
    [SerializeField] private float flameDecayRate = 1f;
    [SerializeField] private Light flameLight;
    
    private float currentFlame;

    public event Action OnFlameDepleted;

    private void Start()
    {
        currentFlame = flameTotal;
    }

    private void Update()
    {
        DecayFlame();
    }

    private void DecayFlame()
    {
        currentFlame -= flameDecayRate * Time.deltaTime;

        if (currentFlame <= 0)
        {
            OnFlameDepleted?.Invoke();
        }
    }
    
    private void UpdateLightIntensity()
    {
        flameLight.intensity = Mathf.Clamp(currentFlame / flameTotal, 0.1f, 1f);
    }
}
