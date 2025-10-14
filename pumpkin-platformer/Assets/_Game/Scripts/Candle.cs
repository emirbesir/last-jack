using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Candle : MonoBehaviour
{
    private const string PLAYER_LAYER_NAME = "Player";

    [Header("Candle Settings")]
    [SerializeField] private float flameReplenishSeconds;
    [SerializeField] private bool isCheckpoint;
    [Header("References")]
    [SerializeField] private Light candleLight;
    [SerializeField] private ParticleSystem candleParticles;

    // References
    private BoxCollider boxCollider;
    private PlayerInputHandler inputHandler;
    private Flame flame;
    private CheckpointManager checkpointManager;

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
        var handler = InputHandler;
        if (handler == null) return;

        handler.OnInteractPerformed += HandleInteract;
    }

    private void OnDisable()
    {
        var handler = InputHandler;
        if (handler == null) return;

        handler.OnInteractPerformed -= HandleInteract;
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
            if (isCheckpoint)
            {
                var checkpointService = CheckpointService;
                var flameService = FlameComponent;

                checkpointService?.SetCheckpoint(transform);
                flameService?.IncreaseMaxFlame(flameReplenishSeconds);
                flameService?.ResetFlame();
            }
            else
            {
                FlameComponent?.ReplenishFlame(flameReplenishSeconds);
            }
            
            candleLight.enabled = false;
            boxCollider.enabled = false;
            hasBeenUsed = true;
            candleParticles.Stop();
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

    private CheckpointManager CheckpointService
    {
        get
        {
            if (checkpointManager == null)
            {
                checkpointManager = CheckpointManager.Instance;
            }

            return checkpointManager;
        }
    }
}
