using System.Collections;
using UnityEngine;

public class ScreenManager : SingletonMonoBehaviour<ScreenManager>
{
    private const string CRT_COLOR_PROPERTY = "_Color";
    private const string GLITCH_STRENGTH_PROPERTY = "_Glitch_Strength";
    private const string GLITCH_NOISE_PROPERTY = "_Noise_Amount";

    [Header("Screen Effects")]
    [SerializeField] private Material CRTMaterial;
    [SerializeField] private Material glitchMaterial;

    [Header("Color Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color startColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color endColor;
    [SerializeField] private float colorTransitionDuration;
    [SerializeField] private float openScreenDelay;

    [Header("Glitch Settings")]
    [Range(0.0f, 100.0f)]
    [SerializeField] private float glitchNoise;
    [Range(0.0f, 100.0f)]
    [SerializeField] private float glitchStrength;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        ClearGlitchEffect();
        ScreenOpeningEffect();
    }

    public void ScreenOpeningEffect()
    {
        StartCoroutine(ScreenOpeningEffectCoroutine());
    }

    public void SetGlitchEffect()
    {
        glitchMaterial.SetFloat(GLITCH_STRENGTH_PROPERTY, glitchStrength);
        glitchMaterial.SetFloat(GLITCH_NOISE_PROPERTY, glitchNoise);
    }

    public void ClearGlitchEffect()
    {
        glitchMaterial.SetFloat(GLITCH_STRENGTH_PROPERTY, 0.0f);
        glitchMaterial.SetFloat(GLITCH_NOISE_PROPERTY, 0.0f);
    }

    private IEnumerator ScreenOpeningEffectCoroutine()
    {
        CRTMaterial.SetColor(CRT_COLOR_PROPERTY, startColor);

        yield return new WaitForSeconds(openScreenDelay);

        float elapsed = 0.0f;

        while (elapsed < colorTransitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / colorTransitionDuration);
            Color currentColor = Color.Lerp(startColor, endColor, t);
            CRTMaterial.SetColor(CRT_COLOR_PROPERTY, currentColor);
            yield return null;
        }

        CRTMaterial.SetColor(CRT_COLOR_PROPERTY, endColor);
    }
    
}
