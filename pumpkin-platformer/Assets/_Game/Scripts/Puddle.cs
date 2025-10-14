using UnityEngine;

public class Puddle : MonoBehaviour
{
    private const string PLAYER_LAYER_NAME = "Player";

    [SerializeField] private float flameDrainRate = 2f;

    private bool isPlayerInPuddle;
    private Flame flame;
    private ScreenManager screenManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER_NAME))
        {
            isPlayerInPuddle = true;
            ScreenService?.SetGlitchEffect();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayerInPuddle)
        {
            FlameComponent?.DamageFlame(flameDrainRate * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER_NAME))
        {
            isPlayerInPuddle = false;
            ScreenService?.ClearGlitchEffect();
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

    private ScreenManager ScreenService
    {
        get
        {
            if (screenManager == null)
            {
                screenManager = ScreenManager.Instance;
            }

            return screenManager;
        }
    }
}
