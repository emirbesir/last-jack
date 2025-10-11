using UnityEngine;
using UnityEngine.UI;

public class FlameViewUI : MonoBehaviour
{
    [SerializeField] private Slider flameSlider;

    private void Update()
    {
        flameSlider.value = Flame.Instance.GetIntensityRatio();
    }
}
