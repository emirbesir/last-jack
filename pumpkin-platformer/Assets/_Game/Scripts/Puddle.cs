using UnityEngine;

public class Puddle : MonoBehaviour
{
    private const string PLAYER_LAYER_NAME = "Player";

    [SerializeField] private float flameDrainRate = 2f;

    private bool isPlayerInPuddle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER_NAME))
        {
            isPlayerInPuddle = true;
            ScreenManager.Instance.SetGlitchEffect();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPlayerInPuddle)
        {
            Flame.Instance.DamageFlame(flameDrainRate * Time.deltaTime);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER_NAME))
        {
            isPlayerInPuddle = false;
            ScreenManager.Instance.ClearGlitchEffect();
        }
    }
}
