using UnityEngine;
using UnityEngine.UI;

public class FlameViewUI : MonoBehaviour
{
    [SerializeField] private Slider flameSlider;

    private Flame flame;

    private void Update()
    {
        var flameComponent = FlameComponent;
        if (flameComponent == null) return;

        flameSlider.value = flameComponent.GetIntensityRatio();
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
