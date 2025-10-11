using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Candle : MonoBehaviour
{   
    private const string PLAYER_LAYER_NAME = "Player";

    [Header("Candle Settings")]
    [SerializeField] private float flameReplenishSeconds;
    [SerializeField] private Light candleLight;

    // References
    private BoxCollider boxCollider;

    // State
    private bool isPlayerInRange;
    private bool hasBeenUsed;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        isPlayerInRange = false;
        hasBeenUsed = false;
    }
    
    private void OnEnable()
    {
        PlayerInputHandler.Instance.OnInteractPerformed += HandleInteract;
    }

    private void OnDisable()
    {
        PlayerInputHandler.Instance.OnInteractPerformed -= HandleInteract;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER_NAME))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(PLAYER_LAYER_NAME))
        {
            isPlayerInRange = false;
        }
    }

    private void HandleInteract()
    {
        if (isPlayerInRange && !hasBeenUsed)
        {
            Flame.Instance.ReplenishFlame(flameReplenishSeconds);
            candleLight.enabled = false;
            boxCollider.enabled = false;
            hasBeenUsed = true;
        }
    }
}
