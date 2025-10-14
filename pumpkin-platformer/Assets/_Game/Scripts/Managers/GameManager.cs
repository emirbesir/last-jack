using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float respawnDelaySeconds;

    [Header("References")]
    [SerializeField] private GameObject player;
    private Rigidbody playerRb;
    private Flame flame;
    private CheckpointManager checkpointManager;
    private ScreenManager screenManager;
    private bool isSubscribedToFlame;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
        SubscribeToFlame();
    }

    private void OnEnable()
    {
        SubscribeToFlame();
    }

    private void OnDisable()
    {
        UnsubscribeFromFlame();
    }

    private void HandleFlameDepleted()
    {
        StartCoroutine(RespawnPlayerAfterDelay(respawnDelaySeconds));
    }

    private IEnumerator RespawnPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Reset
        FlameComponent?.ResetFlame();
        CheckpointService?.RespawnPlayer();

        // Reset player velocity to prevent carry-over momentum
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;
        
        ScreenService?.ScreenOpeningEffect();
    }

    private void SubscribeToFlame()
    {
        if (isSubscribedToFlame)
        {
            return;
        }

        var flameComponent = FlameComponent;
        if (flameComponent == null)
        {
            return;
        }

        flameComponent.OnFlameDepleted += HandleFlameDepleted;
        isSubscribedToFlame = true;
    }

    private void UnsubscribeFromFlame()
    {
        if (!isSubscribedToFlame)
        {
            return;
        }

        var flameComponent = FlameComponent;
        if (flameComponent == null)
        {
            isSubscribedToFlame = false;
            return;
        }

        flameComponent.OnFlameDepleted -= HandleFlameDepleted;
        isSubscribedToFlame = false;
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
