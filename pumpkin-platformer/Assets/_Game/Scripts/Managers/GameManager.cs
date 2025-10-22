using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float respawnDelaySeconds;

    [Header("References")]
    [SerializeField] private GameObject player;
    private Rigidbody playerRb;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Flame.Instance.OnFlameDepleted += HandleFlameDepleted;
    }
    
    private void OnDisable()
    {
        Flame.Instance.OnFlameDepleted -= HandleFlameDepleted;
    }

    private void HandleFlameDepleted()
    {
        StartCoroutine(RespawnPlayerAfterDelay(respawnDelaySeconds));
    }

    private IEnumerator RespawnPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Reset player velocity to prevent carry-over momentum
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;
        
        // Reset
        CheckpointManager.Instance.RespawnPlayer();
        Flame.Instance.ResetFlame();

        
        ScreenManager.Instance.ScreenOpeningEffect();
    }
}
